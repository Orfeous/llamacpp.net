using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Models;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Native.API;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers.Abstractions;
using LlamaCpp.Net.Samplers.Pipelines;
using Microsoft.Extensions.Logging;

namespace LlamaCpp.Net;
#pragma warning disable CA1848

/// <inheritdoc />
public class LanguageModel : ILanguageModel
{
    private readonly Encoding _encoding = Encoding.UTF8;
    private static int EndOfSequenceToken => LlamaNative.llama_token_eos();
    private readonly ILlamaInstance _instance;
    private readonly ILogger<LanguageModel> _logger;
    private readonly Dictionary<int, string> _tokenCache = new();
    private readonly int _vocabSize;

    private int[] _embeds = Array.Empty<int>();

    /// <summary>
    ///     The constructor for the language model
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <param name="samplingPipeline"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public LanguageModel(string modelPath, ILogger<LanguageModel> logger, LanguageModelOptions? options = null)
    {
        _logger = logger;
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
    }

    internal LanguageModel(ISamplingPipeline constraints, ILlamaInstance instance, ILogger<LanguageModel> logger,
        string modelPath, LanguageModelOptions options)
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
    public List<int> Tokenize(string text)
    {
        _logger.LogDebug("Tokenizing text {Text}", text);

        var tokens = new int[text.Length + 1];

        var numberOfTokens = _instance.Tokenize(text, tokens, tokens.Length, true);

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

                _instance.PrintTimings();
                result.Complete();
            }, cancellationToken);

        return result.ToAsyncEnumerable(cancellationToken);
    }

    /// <inheritdoc />
    public IEnumerable<string> Infer(string input, InferenceOptions? options = null)
    {
        _logger.LogDebug("Starting inference for input {Input}", input);
        var opts = options ?? InferenceOptions.Default;
        var result = GenerateModelOutput(input, opts);

        _logger.LogDebug("Inference for input {Input} completed", input);
        return result;
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

            EvaluatePrompt(options.InitialPrompt);
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
    private List<string> GenerateModelOutput(string input, InferenceOptions options,
        Action<string>? inferenceCallback = null)
    {
        if (!string.IsNullOrEmpty(options.PromptPrefix))
        {
            _logger.LogDebug("Adding prompt prefix {PromptPrefix} to input", options.PromptPrefix);
            input = options.PromptPrefix + input;
        }

        if (!string.IsNullOrEmpty(options.PromptSuffix))
        {
            _logger.LogDebug("Adding prompt suffix {PromptSuffix} to input", options.PromptSuffix);
            input += options.PromptSuffix;
        }

        EvaluatePrompt(input);

        var list = new List<string>();


        var constraints = BuildConstraints(options).Build(_instance);

        var logits = GetLogits(_instance, _vocabSize);

        var currentOutput = Array.Empty<int>();
        for (var i = 0; i < options.MaxNumberOfTokens; i++)
        {
            var candidates = GetCandidates(_vocabSize, logits);

            var id = constraints.ApplyConstraints(candidates, logits, currentOutput, options.PenalizeNewLine,
                options.RepetitionLookback);


            if (id == EndOfSequenceToken)
            {
                break;
            }

            var embedsInCurrentBatch = new[] { id };
            _embeds = _embeds.Concat(embedsInCurrentBatch).ToArray();

            currentOutput = currentOutput.Concat(embedsInCurrentBatch).ToArray();

            if (inferenceCallback != null)
            {
                var s = TokenToString(id);
                inferenceCallback?.Invoke(s);
            }

            if (_instance.Eval(embedsInCurrentBatch, 1, _embeds.Length, Threads) != 0)
            {
                _logger.LogError("Failed to evaluate model");

                throw new ArgumentException("Failed to evaluate model");
            }
        }

        _logger.LogDebug("Inference completed");
        _logger.LogDebug("Output: {Output}", string.Join("", list));

        return list;
    }

    private ISamplingPipelineBuilder BuildConstraints(
        InferenceOptions options)
    {
        ISamplingPipelineBuilder constraints = new SamplingPipelineBuilder(_logger);

        if (options.Temperature < 0)
        {
            // should set sampling method to greedy

            return constraints;
        }
        else
        {
            if (options.SamplingMethod == SamplingMethod.Mirostat)
            {
                constraints = constraints.AddTemperatureSampler(options.Temperature)
                    .SetEndSampler(SamplingMethod.Mirostat);
            }
            else if (options.SamplingMethod == SamplingMethod.MirostatV2)
            {
                constraints = constraints.AddTemperatureSampler(options.Temperature)
                    .SetEndSampler(SamplingMethod.MirostatV2);
            }
            else
            {
                constraints = constraints
                    .AddTopKSampler(options.TopK, options.TopKMinKeep)
                    .AddTailFreeSampler(options.TailFreeZ, options.TailFreeMinKeep)
                    .AddTypicalSampler(options.LocalTypicalK, options.LocalTypicalMinKeep)
                    .AddTopPSampler(options.TopP, options.TopPMinKeep)
                    .AddTemperatureSampler(options.Temperature)
                    .SetEndSampler(SamplingMethod.Default);
            }
        }

        return constraints;
    }


    private void EvaluatePrompt(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return;
        }


        _logger.LogDebug("Evaluating prompt {Prompt}", input);

        var embeds = new int[input.Length + 1];

        var amountOfTokens = _instance.Tokenize(input, embeds, embeds.Length, true);
        Array.Resize(ref embeds, amountOfTokens);

        if (embeds.Where((t, i) => _instance.Eval(new[] { t }, 1, i, Threads) != 0).Any())
        {
            _logger.LogError("Failed to evaluate model");

            throw new ArgumentException("Failed to evaluate model");
        }

        _embeds = _embeds.Concat(embeds).ToArray();
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