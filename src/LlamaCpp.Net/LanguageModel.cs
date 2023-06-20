using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Models;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaCpp.Net;

/// <inheritdoc />
public class LanguageModel : ILanguageModel
{
    private readonly ModelConstraints _constraints;
    private readonly SafeLLamaContextHandle _contextHandle;
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly int _endOfSequenceToken;
    private readonly ILogger<LanguageModel> _logger;

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
                if (File.Exists(options.LoraAdapterPath) == false)
                    throw new FileNotFoundException($"Lora adapter file {options.LoraAdapterPath} not found",
                        options.LoraAdapterPath);
        }

        Options = options;


        var contextParams = LlamaNative.llama_context_default_params();

        contextParams.Apply(options);
        contextParams.progress_callback = ProgressCallback;

        IntPtr context;
        try
        {
            context = LlamaNative.llama_init_from_file(modelPath, contextParams);
        }
        catch (SEHException e)
        {
            throw new ModelFailedInitializationException(modelPath, e);
        }

        if (context == IntPtr.Zero) throw new ModelFailedInitializationException(modelPath);

        var handle = new SafeLLamaContextHandle(context);

        if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
            InitializeLora(handle, modelPath, options.LoraAdapterPath, options.LoraThreads);

        _contextHandle = handle;

        _endOfSequenceToken = LlamaNative.llama_token_eos();

        _constraints = new ModelConstraints(_contextHandle);
    }


    /// <inheritdoc />

    public string ModelPath { get; init; }

    /// <inheritdoc />

    public LanguageModelOptions Options { get; init; }

    /// <inheritdoc />
    public List<int> Tokenize(string text)
    {
        var tokens = new int[text.Length + 1];

        var numberOfTokens = _contextHandle.llama_tokenize(text, tokens, tokens.Length, true);

        if (numberOfTokens == 0) throw new TokenizationFailedException(text);

        Array.Resize(ref tokens, numberOfTokens);

        return tokens.Take(numberOfTokens).ToList();
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
        var ptr = _contextHandle.llama_token_to_str(token);
        return ptr.PtrToString(_encoding);
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

        Task.Run(() => { GenerateModelOutput(input, options, s => { result.Append(s); }); }, cancellationToken)
            .ContinueWith(_ => { result.Complete(); }, cancellationToken);

        return result.ToAsyncEnumerable(cancellationToken);
    }

    /// <inheritdoc />
    public IEnumerable<string> Infer(string input, InferenceOptions? options = null)
    {
        var opts = options ?? InferenceOptions.Default;
        return this.GenerateModelOutput(input, opts);
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
        if (!string.IsNullOrWhiteSpace(Options.PromptPrefix))
        {
            input = Options.PromptPrefix + input;

        }

        if (!string.IsNullOrWhiteSpace(Options.PromptSuffix))
        {
            input += Options.PromptSuffix;
        }

        var inputTokens = Tokenize(input).ToArray();

        var outputTokens = new List<int>();
        outputTokens.AddRange(inputTokens);
        for (var i = 0; i < options.MaxNumberOfTokens; i++)
        {
            var tokens = outputTokens.ToArray();
            TryEvaluate(options, tokens, out var evalResult);
            if (!evalResult) break;

            var vocabSize = GetVocab(out var logits);

            var candidatesP = GetCandidates(vocabSize, logits);


            _constraints.ApplyConstraints(candidatesP, outputTokens, logits, options);

            TemperatureSampler.CreateInstance(_contextHandle, options.Temperature).Sample(candidatesP);
            var id = _contextHandle.SampleGreedy(candidatesP);
            var s = TokenToString(id);
            Trace.WriteLine(s);
            if (id == _endOfSequenceToken)
            {
                Console.WriteLine("EOS");
                break;
            }

            outputTokens.Add(id);

            inferenceCallback?.Invoke(s);
        }

        return outputTokens.Select(TokenToString).ToList();
    }

    /// <summary>
    ///     Gets the vocabulary size and logits for the current language model.
    /// </summary>
    /// <param name="logits">A span of floats representing the logits for each token in the vocabulary.</param>
    /// <param name="logitBias">An optional dictionary of token IDs and their corresponding bias values to add to the logits.</param>
    /// <returns>The size of the vocabulary.</returns>
    private int GetVocab(out Span<float> logits, Dictionary<int, float>? logitBias = null)
    {
        var vocabSize = _contextHandle.llama_n_vocab();
        logits = GetLogits(_contextHandle, vocabSize);

        if (logitBias is null) return vocabSize;

        foreach (var (key, value) in logitBias) logits[key] += value;

        return vocabSize;
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
    /// <param name="result">The evaluation result.</param>
    private void TryEvaluate(InferenceOptions options, int[] tokens, out bool result)
    {
        var evalResult = _contextHandle.llama_eval(tokens, tokens.Length, tokens.Length,
            options.Threads);
        result = evalResult == 0;
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