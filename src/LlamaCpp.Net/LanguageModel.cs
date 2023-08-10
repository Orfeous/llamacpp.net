using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Models;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Native.API;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers.Pipelines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaCpp.Net;
#pragma warning disable CA1848

/// <inheritdoc />
public class LanguageModel : ILanguageModel
{
    private readonly Encoding _encoding = Encoding.UTF8;
    private static int EndOfSequenceToken => LlamaNative.llama_token_eos();
    private static int BeginOfSequenceToken => LlamaNative.llama_token_bos();
    private readonly ILlamaInstance _instance;
    private readonly ILogger<LanguageModel> _logger;
    private readonly Dictionary<int, string> _tokenCache = new Dictionary<int, string>();
    private readonly int _vocabSize;

    private int[] _currentContext = Array.Empty<int>();
    private readonly int _contextSize;

    /// <summary>
    ///     The constructor for the language model
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public LanguageModel(string modelPath, LanguageModelOptions? options = null,
        ILogger<LanguageModel>? logger = null)
    {
        _logger = logger ?? new NullLogger<LanguageModel>();
        ModelPath = modelPath;


        if (File.Exists(ModelPath) == false)
        {
            throw new FileNotFoundException($"Model file {modelPath} not found", modelPath);
        }

        options ??= LanguageModelOptions.Default;

        Options = options;

        var contextParams = GetContextParams(options);

        _instance = new LlamaInstance(modelPath, contextParams);
        if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
        {
            InitializeLora(_instance, modelPath, options.LoraAdapterPath, options.LoraThreads);
        }


        _vocabSize = _instance.GetVocabSize();


        _tokenCache = new Dictionary<int, string>(_vocabSize);

        InitializeTokenCache();

        LoadInitialPrompt(options);

        _contextSize = _instance.GetContextSize();
    }

    internal LanguageModel(ILlamaInstance instance, ILogger<LanguageModel> logger,
        string modelPath)
    {
        _instance = instance;
        _logger = logger;
        ModelPath = modelPath;
        Options = LanguageModelOptions.Default;
        _vocabSize = _instance.GetVocabSize();
    }


    /// <summary>
    /// </summary>
    private int Threads => Options.Threads;


    /// <inheritdoc />

    public string ModelPath { get; init; }

    /// <inheritdoc />

    public LanguageModelOptions Options { get; init; }

    /// <inheritdoc />
    public List<int> Tokenize(string text, bool addBos = true)
    {
        _logger.LogDebug("Tokenizing text {Text}", text);

        var tokens = new int[text.Length + 1];

        var numberOfTokens = _instance.Tokenize(text, tokens, tokens.Length, addBos);

        if (numberOfTokens == 0)
        {
            _logger.LogError("Failed to tokenize text {Text}", text);
            throw new TokenizationFailedException(text);
        }

        Array.Resize(ref tokens, numberOfTokens);

        var result = tokens.Take(numberOfTokens).ToList();

        _logger.LogDebug("Tokenized text {Text} to {Tokens}", text, result);

        return result;
    }


    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public string TokenToString(int token)
    {
        if (_tokenCache.TryGetValue(token, out var toString))
        {
            return toString;
        }

        _logger.LogDebug("Converting token {Token} to string", token);


        _tokenCache[token] = _instance.TokenToString(token, _encoding);
        return _tokenCache[token];
    }

    /// <summary>
    ///     Infers the next tokens given an input string.
    /// </summary>
    /// <param name="input">The input string to infer from.</param>
    /// <param name="options">The inference options to use.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>An enumerable of the inferred tokens as strings.</returns>
    /// <inheritdoc />
    public IAsyncEnumerable<string> InferAsync(string input, InferenceOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = new AsyncInferenceResult();
        options ??= InferenceOptions.Default;

        Task.Run(() =>
            {
                _logger.LogDebug("Starting async inference for input {Input}", input);

                GenerateModelOutput(input, options, s => { result.Append(s); });
            }, cancellationToken)
            .ContinueWith(_ =>
            {
                _logger.LogDebug("Async inference for input \"{Input}\" completed", input);

                result.Complete();
            }, cancellationToken);

        return result.ToAsyncEnumerable(cancellationToken);
    }

    /// <inheritdoc />
    public string Infer(string input, InferenceOptions? options = null)
    {
        _logger.LogDebug("Starting inference for input {Input}", input);
        var opts = options ?? InferenceOptions.Default;

        var sb = new StringBuilder();
        GenerateModelOutput(input, opts, s => sb.Append(s));

        _logger.LogDebug("Inference for input {Input} completed", input);
        return sb.ToString();
    }

    private static LLamaContextParams GetContextParams(LanguageModelOptions options)
    {
        var contextParams = LlamaNative.llama_context_default_params();

        contextParams.Apply(options);
        contextParams.progress_callback = ProgressCallback;
        return contextParams;
    }

    /// <summary>
    ///     Initializes the token cache so that we can quickly look up tokens by their index
    ///     this reduces the number of calls to the native library
    /// </summary>
    private void InitializeTokenCache()
    {
        _logger.LogDebug("Caching tokens");
        for (var i = 0; i < _vocabSize; i++)
        {
            var token = _instance.TokenToString(i, _encoding);
            _tokenCache.Add(i, token);
        }
    }

    /// <summary>
    ///     Initializes the model with an initial prompt if one is provided
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="InitialPromptTooLongException"></exception>
    private void LoadInitialPrompt(LanguageModelOptions options)
    {
        var contextSize = _instance.GetContextSize();

        if (!string.IsNullOrWhiteSpace(options.InitialPrompt))
        {
            _logger.LogDebug("Tokenizing initial prompt {InitialPrompt}", options.InitialPrompt);
            var tokens = Tokenize(options.InitialPrompt).ToArray();

            if (tokens.Length > contextSize)
            {
                _logger.LogWarning("Initial prompt {InitialPrompt} is longer than context size {ContextSize}",
                    options.InitialPrompt, contextSize);
                throw new InitialPromptTooLongException(contextSize);
            }

            EvaluatePrompt(options.InitialPrompt, InferenceOptions.Default);
        }

        else
        {
            _logger.LogDebug("No initial prompt provided");
        }
    }


    /// <summary>
    ///     Infers the next tokens given an input string.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="input"></param>
    /// <param name="inferenceCallback"></param>
    /// <returns></returns>
    private void GenerateModelOutput(string input, InferenceOptions options,
        Action<string> inferenceCallback)
    {
        _logger.LogInformation(
            "sampling: " +
            "repeat_last_n = {RepeatLastN}, " +
            "repeat_penalty = {RepeatPenalty}, " +
            "presence_penalty = {PresencePenalty}, " +
            "frequency_penalty = {FrequencyPenalty}, " +
            "top_k = {TopK}, " +
            "tfs_z = {TfsZ}, " +
            "top_p = {TopP}, " +
            "typical_p = {TypicalP}, " +
            "temp = {Temp}, " +
            "mirostat_lr = {MirostatTau}, " +
            "mirostat_ent = {MirostatEta}",
            options.RepeatLastN, options.RepeatPenalty, options.PresencePenalty, options.FrequencyPenalty, options.TopK,
            options.TailFreeZ, options.TopP, options.LocalTypicalK, options.Temperature,
            options.MirostatTau, options.MirostatEta);


        _logger.LogInformation("generate: n_ctx = {ContextSize}, " +
                               "n_batch = {BatchSize}, " +
                               "n_predict = {PredictSize}, " +
                               "n_keep = {KeepSize}",
            _contextSize, options.BatchSize, options.MaxNumberOfTokens, options.KeepSize);

        input = ApplyPromptModifications(input, options);

        EvaluatePrompt(input, options);

        var tokenizedAntiPrompts = options.Antiprompts.Select(s => Tokenize(s, false)).ToArray();

        var constraints = new SamplingPipeline(this._instance, _logger);

        var logits = GetLogits(_instance, _vocabSize);

        var currentOutput = Array.Empty<int>();


        while (currentOutput.Length < options.MaxNumberOfTokens || options.MaxNumberOfTokens == -1)
        {
            var candidates = GetCandidates(_vocabSize, logits);

            var id = constraints.ApplyConstraints(candidates, logits, currentOutput, options);


            if (id == EndOfSequenceToken)
            {
                _logger.LogDebug("End of sequence token found");
                break;
            }

            var embedsInCurrentBatch = new[] { id };
            _currentContext = _currentContext.Concat(embedsInCurrentBatch).ToArray();

            currentOutput = currentOutput.Concat(embedsInCurrentBatch).ToArray();


            var antiPromptFound = false;
            // TODO : figure out a way to defer the callback until know for sure that it does not contain an antiprompt.
            // Currently, the callback is called before the antiprompt is removed from the output, which may result in partial antiprompts being returned.
            foreach (var p in tokenizedAntiPrompts)
            {

                if (p.Count > currentOutput.Length)
                {
                    continue;
                }

                var antiprompt = _currentContext.Skip(_currentContext.Length - p.Count).ToArray();

                if (!antiprompt.SequenceEqual(p))
                {
                    continue;
                }

                _logger.LogDebug("Antiprompt found");

                // remove antiprompt from current output

                currentOutput = currentOutput.Take(currentOutput.Length - p.Count).ToArray();


                antiPromptFound = true;
                break;
            }

            if (antiPromptFound)
            {
                break;
            }


            if (_instance.Eval(embedsInCurrentBatch, 1, _currentContext.Length, Threads) == 0)
            {
                // success
                continue;
            }

            _logger.LogError("Failed to evaluate model");

            throw new ArgumentException("Failed to evaluate model");
        }

        foreach (var token in currentOutput)
        {
            if (token == EndOfSequenceToken || token == BeginOfSequenceToken)
            {
                continue;
            }

            var s = TokenToString(token);
            if (token == currentOutput.First())
            {
                s = s.TrimStart();
            }

            if (token == currentOutput.Last())
            {
                s = s.TrimEnd();
            }

            inferenceCallback?.Invoke(s);
        }

        _logger.LogDebug("Inference completed: {Output}", currentOutput);
    }

    private string ApplyPromptModifications(string input, InferenceOptions options)
    {
        if (!string.IsNullOrEmpty(options.PromptPrefix))
        {
            _logger.LogDebug("Adding prompt prefix {PromptPrefix} to input", options.PromptPrefix);
            input = options.PromptPrefix + input;
        }
        else
        {
            _logger.LogDebug("No prompt prefix provided");
        }

        if (!string.IsNullOrEmpty(options.PromptSuffix))
        {
            _logger.LogDebug("Adding prompt suffix {PromptSuffix} to input", options.PromptSuffix);
            input += options.PromptSuffix;
        }
        else
        {
            _logger.LogDebug("No prompt suffix provided");
        }

        return input.Trim();
    }

    private void EvaluatePrompt(string input, InferenceOptions inferenceOptions)
    {
        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        _logger.LogDebug("Evaluating prompt {Prompt}", input);

        var embeds = new int[input.Length + 1];

        var amountOfTokens = _instance.Tokenize(input, embeds, embeds.Length, true);

        var batchSize = inferenceOptions.BatchSize;


        Array.Resize(ref embeds, amountOfTokens);

        var contextLength = Math.Min(_contextSize, _currentContext.Length) - 1;
        if (contextLength < 0)
        {
            contextLength = 0;
        }

        _instance.Eval(embeds, embeds.Length, contextLength, Threads);

        _currentContext = _currentContext.Concat(embeds).ToArray();
    }


    /// <summary>
    ///     Returns an array of token candidates based on the given logits.
    /// </summary>
    /// <param name="vocabSize">The size of the vocabulary.</param>
    /// <param name="logits">A span of floats representing the logits for each token in the vocabulary.</param>
    /// <returns>An array of token candidates.</returns>
    private static TokenDataArray GetCandidates(int vocabSize, Span<float> logits)
    {
        var candidates = new List<TokenData> { Capacity = vocabSize };
        for (var tokenId = 0; tokenId < vocabSize; tokenId++)
            candidates.Add(new TokenData(tokenId, logits[tokenId], 0.0f));

        var candidatesP = new TokenDataArray(candidates.ToArray(), (ulong)candidates.Count, false);
        return candidatesP;
    }


    /// <summary>
    ///     Gets a span of floats representing the logits for each token in the vocabulary from the current language model.
    /// </summary>
    /// <param name="contextHandle">The handle to the current language model context.</param>
    /// <param name="length">The length of the span to return.</param>
    /// <returns>A span of floats representing the logits for each token in the vocabulary.</returns>
    private static unsafe Span<float> GetLogits(ILlamaInstance contextHandle, int length)
    {
        var logits = contextHandle.GetLogits();
        return new Span<float>(logits, length);
    }


    private static void ProgressCallback(float progress, IntPtr ctx)
    {
    }

    /// <summary>
    ///     Initializes the LORA adapter for the given language model context handle.
    /// </summary>
    /// <param name="contextHandle">The handle to the language model context.</param>
    /// <param name="modelPath">The path to the language model file.</param>
    /// <param name="loraAdapterPath">The path to the LORA adapter file.</param>
    /// <param name="optionsLoraThreads">The number of threads to use for LORA inference.</param>
    private void InitializeLora(ILlamaInstance contextHandle, string modelPath,
        string loraAdapterPath, int optionsLoraThreads)
    {
        if (File.Exists(loraAdapterPath) == false)
        {
            throw new FileNotFoundException($"Lora adapter file {loraAdapterPath} not found",
                loraAdapterPath);
        }

        _logger.LogDebug("Initializing Lora adapter from file {LoraAdapterPath}", loraAdapterPath);
        var r = contextHandle.ApplyLoraFromFile(loraAdapterPath, modelPath,
            optionsLoraThreads);

        if (r != 0)
        {
            throw new LoraAdapterFailedInitializationException(modelPath, loraAdapterPath,
                optionsLoraThreads);
        }
    }

    /// <summary>
    ///     Dispose of the context handle
    /// </summary>
    /// <param name="isDisposing"></param>
    private void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _instance.Dispose();
        }
    }
}

#pragma warning restore CA1848