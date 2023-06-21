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
    private readonly int _vocabSize;
    private readonly int _newLineToken;
    private readonly Dictionary<int, string> _tokenCache;

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
            throw new FileNotFoundException($"Model file {modelPath} not found", modelPath);

        if (options == null)
        {
            options = LanguageModelOptions.Default;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
            {
                if (File.Exists(options.LoraAdapterPath) == false)
                {
                    throw new FileNotFoundException($"Lora adapter file {options.LoraAdapterPath} not found",
                        options.LoraAdapterPath);
                }
            }
        }

        Options = options;


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

        if (context == IntPtr.Zero) throw new ModelFailedInitializationException(modelPath);

        var handle = new SafeLLamaContextHandle(context);

        if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
        {
            _logger.LogDebug("Initializing Lora adapter from file {LoraAdapterPath}", options.LoraAdapterPath);
            InitializeLora(handle, modelPath, options.LoraAdapterPath, options.LoraThreads);
        }

        _contextHandle = handle;

        _endOfSequenceToken = LlamaNative.llama_token_eos();
        _vocabSize = _contextHandle.llama_n_vocab();
        _newLineToken = LlamaNative.llama_token_nl();

        _logger.LogDebug("Initializing constraints");
        _constraints = new ModelConstraints(_contextHandle, logger);

        this._tokenCache = new Dictionary<int, string>(_vocabSize);

        _logger.LogDebug("Caching tokens");
        for (var i = 0; i < _vocabSize; i++)
        {
            var token = _contextHandle.llama_token_to_str(i);
            _tokenCache.Add(i, token.PtrToString(_encoding));
        }
    }


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
                result.Complete();
            }, cancellationToken);

        return result.ToAsyncEnumerable(cancellationToken);
    }

    /// <inheritdoc />
    public IEnumerable<string> Infer(string input, InferenceOptions? options = null)
    {
        _logger.LogDebug("Starting inference for input {Input}", input);
        var opts = options ?? InferenceOptions.Default;
        var result = this.GenerateModelOutput(input, opts);

        _logger.LogDebug("Inference for input {Input} completed", input);
        return result;
    }


    /// <summary>
    ///    Infers the next tokens given an input string.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="input"></param>
    /// <param name="inferenceCallback"></param>
    /// <returns></returns>
    private List<string> GenerateModelOutput(string input, InferenceOptions options,
        Action<string>? inferenceCallback = null)
    {
        // for some reason, the models like it better if we add a space before the first token
        // silly models

        _logger.LogDebug("Generating model output for input {Input}", input);
        input = " " + input;
        if (!string.IsNullOrWhiteSpace(Options.PromptPrefix))
        {
            input = Options.PromptPrefix + input;
        }

        if (!string.IsNullOrWhiteSpace(Options.PromptSuffix))
        {
            input += Options.PromptSuffix;
        }

        var inputTokens = Tokenize(input).ToArray();
        var logits = GetLogits(_contextHandle, _vocabSize);

        var lastTokens = new List<int>();
        lastTokens.AddRange(inputTokens);
        var evaluatedTokens = 0;
        for (var i = 0; i < options.MaxNumberOfTokens; i++)
        {
            var tokens = lastTokens.ToArray();
            evaluatedTokens = Evaluate(options, tokens, evaluatedTokens);


            var candidatesP = GetCandidates(_vocabSize, logits);


            var selectedToken = _constraints.ApplyConstraints(candidatesP, lastTokens, logits, options);
            if (selectedToken == _endOfSequenceToken)
            {
                _logger.LogDebug("End of sequence token found");
                break;
            }

            if (options.TreatNewLineAsEndOfText)
            {
                if (selectedToken == _newLineToken)
                {
                    _logger.LogDebug("New line token found");
                    break;
                }
            }

            lastTokens.Add(selectedToken);

            if (inferenceCallback != null)
            {
                var s = TokenToString(selectedToken);
                inferenceCallback?.Invoke(s);
            }
        }

        return lastTokens.Select(TokenToString).ToList();
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
    ///     Tries to evaluate the given tokens using the current language model and inference options.
    /// </summary>
    /// <param name="options">The inference options to use.</param>
    /// <param name="tokens">The tokens to evaluate.</param>
    /// <param name="evaluatedTokens"></param>
    private int Evaluate(InferenceOptions options, int[] tokens, int evaluatedTokens)
    {
        var total = tokens.Length;

        if (_contextHandle.llama_eval(tokens, total, evaluatedTokens,
                options.Threads) != 0)
        {
            throw new ArgumentException("Evaluation failed ");
        }

        evaluatedTokens += tokens.Length;


        return evaluatedTokens;
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
    private static void InitializeLora(SafeLLamaContextHandle contextHandle, string modelPath,
        string loraAdapterPath, int optionsLoraThreads)
    {
        var r = contextHandle.llama_apply_lora_from_file(loraAdapterPath, modelPath,
            optionsLoraThreads);

        if (r != 0)
            throw new LoraAdapterFailedInitializationException(modelPath, loraAdapterPath,
                optionsLoraThreads);
    }

    /// <summary>
    ///     Dispose of the context handle
    /// </summary>
    /// <param name="isDisposing"></param>
    private void Dispose(bool isDisposing)
    {
        if (isDisposing) _contextHandle.Dispose();
    }
}
#pragma warning restore CA1848