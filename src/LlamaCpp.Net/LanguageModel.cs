using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Models;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaCpp.Net;
#pragma warning disable CA1848

/// <inheritdoc />
public class LanguageModel : ILanguageModel
{
    private readonly ModelConstraints _constraints;
    private readonly SafeLLamaContextHandle _contextHandle;
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly int _endOfSequenceToken;
    private readonly ILogger<LanguageModel> _logger;
    private readonly Dictionary<int, string> _tokenCache;
    private readonly int _vocabSize;

    private int[] _embeds = Array.Empty<int>();

    /// <summary>
    ///     The constructor for the language model
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="logger"></param>
    /// <param name="options"></param>
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
        Threads = options.Threads;

        var contextParams = LlamaNative.llama_context_default_params();

        contextParams.Apply(options);
        contextParams.progress_callback = ProgressCallback;

        IntPtr context;
        try
        {
            _logger.LogDebug("Initializing model from file {ModelPath}", modelPath);
            context = LlamaNative.llama_init_from_file(modelPath, contextParams);
        }
        catch (SEHException e)
        {
            _logger.LogError(e, "Failed to initialize model from file {ModelPath}", modelPath);
            throw new ModelFailedInitializationException(modelPath, e);
        }

        if (context == IntPtr.Zero)
        {
            throw new ModelFailedInitializationException(modelPath);
        }

        var handle = new SafeLLamaContextHandle(context);

        if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
        {
            InitializeLora(handle, modelPath, options.LoraAdapterPath, options.LoraThreads);
        }

        _contextHandle = handle;

        _endOfSequenceToken = LlamaNative.llama_token_eos();
        _vocabSize = _contextHandle.llama_n_vocab();
        var contextSize = _contextHandle.llama_n_ctx();
        _logger.LogDebug("Initializing constraints");
        _constraints = new ModelConstraints(_contextHandle, logger);

        _tokenCache = new Dictionary<int, string>(_vocabSize);

        InitializeTokenCache();

        LoadInitialPrompt(options, contextSize);
    }

    /// <summary>
    /// </summary>
    private int Threads { get; }


    /// <inheritdoc />

    public string ModelPath { get; init; }

    /// <inheritdoc />

    public LanguageModelOptions Options { get; init; }

    /// <inheritdoc />
    public List<int> Tokenize(string text)
    {
        _logger.LogDebug("Tokenizing text {Text}", text);

        var tokens = new int[text.Length + 1];

        var numberOfTokens = _contextHandle.llama_tokenize(text, tokens, tokens.Length, true);

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
        var ptr = _contextHandle.llama_token_to_str(token);
        var s = ptr.PtrToString(_encoding);

        _tokenCache[token] = s;
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
                _logger.LogDebug("Async inference for input {Input} completed", input);

                _contextHandle.llama_print_timings();
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

    /// <summary>
    ///     Prints system info to the log.
    /// </summary>
    public void PrintSystemInfo()
    {
        LlamaNative.llama_print_system_info();
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
            var token = _contextHandle.llama_token_to_str(i);
            _tokenCache.Add(i, token.PtrToString(_encoding));
        }
    }

    /// <summary>
    ///     Initializes the model with an initial prompt if one is provided
    /// </summary>
    /// <param name="options"></param>
    /// <param name="contextSize"></param>
    /// <exception cref="InitialPromptTooLongException"></exception>
    private void LoadInitialPrompt(LanguageModelOptions options, int contextSize)
    {
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
        EvaluatePrompt(input);

        var list = new List<string>();

        var logits = GetLogits(_contextHandle, _vocabSize);

        for (var i = 0; i < options.MaxNumberOfTokens; i++)
        {
            var candidates = GetCandidates(_vocabSize, logits);

            var id = _constraints.ApplyConstraints(candidates, logits, options);


            if (id == _endOfSequenceToken)
            {
                break;
            }

            var newEmbeds = new[] { id };
            _embeds = _embeds.Concat(newEmbeds).ToArray();


            if (inferenceCallback != null)
            {
                var s = TokenToString(id);
                inferenceCallback?.Invoke(s);
            }

            if (_contextHandle.llama_eval(newEmbeds, 1, _embeds.Length, Threads) != 0)
            {
                _logger.LogError("Failed to evaluate model");

                throw new ArgumentException("Failed to evaluate model");
            }
        }


        return list;
    }

    private void EvaluatePrompt(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        if (!string.IsNullOrEmpty(Options.PromptPrefix))
        {
            input = Options.PromptPrefix + input;
        }

        if (!string.IsNullOrEmpty(Options.PromptSuffix))
        {
            input += Options.PromptSuffix;
        }

        _logger.LogDebug("Evaluating prompt {Prompt}", input);

        var embeds = new int[input.Length + 1];

        var amountOfTokens = _contextHandle.llama_tokenize(input, embeds, embeds.Length, true);
        Array.Resize(ref embeds, amountOfTokens);

        if (embeds.Where((t, i) => _contextHandle.llama_eval(new[] { t }, 1, i, Threads) != 0).Any())
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
    private static unsafe Span<float> GetLogits(SafeLLamaContextHandle contextHandle, int length)
    {
        var logits = contextHandle.llama_get_logits();
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
    private void InitializeLora(SafeLLamaContextHandle contextHandle, string modelPath,
        string loraAdapterPath, int optionsLoraThreads)
    {
        if (File.Exists(loraAdapterPath) == false)
        {
            throw new FileNotFoundException($"Lora adapter file {loraAdapterPath} not found",
                loraAdapterPath);
        }

        _logger.LogDebug("Initializing Lora adapter from file {LoraAdapterPath}", loraAdapterPath);
        var r = contextHandle.llama_apply_lora_from_file(loraAdapterPath, modelPath,
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
            _contextHandle.Dispose();
        }
    }
}

#pragma warning restore CA1848