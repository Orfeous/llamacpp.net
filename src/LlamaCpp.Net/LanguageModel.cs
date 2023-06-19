using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Native;
using Microsoft.Extensions.Logging;

namespace LlamaCpp.Net
{
    /// <inheritdoc />
    public class LanguageModel : ILanguageModel
    {
        private readonly ModelConstraints _constraints;
        private readonly SafeLLamaContextHandle _contextHandle;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly int _eosToken;
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
            {
                throw new FileNotFoundException($"Model file {modelPath} not found", modelPath);
            }

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
                context = LlamaNative.llama_init_from_file(modelPath, contextParams);
            }
            catch (SEHException e)
            {
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

            _eosToken = LlamaNative.llama_token_eos();
            var nlToken = LlamaNative.llama_token_nl();
            var contextSize = _contextHandle.llama_n_ctx();

            _constraints = new ModelConstraints(_contextHandle, nlToken, contextSize);
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

            if (numberOfTokens == 0)
            {
                throw new TokenizationFailedException(text);
            }

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

        /// <inheritdoc />
        public IEnumerable<string> InferAsync(string input, InferenceOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= InferenceOptions.Default;

            var inputTokens = Tokenize(input).ToArray();


            var outputTokens = new List<int>();
            outputTokens.AddRange(inputTokens);

            for (var i = 0; i < options.MaxNumberOfTokens; i++)
            {
                var tokens = outputTokens.ToArray();
                TryEvaluate(options, tokens, out var evalResult);
                if (!evalResult)
                {
                    break;
                }

                var vocabSize = GetVocab(out var logits);

                var candidatesP = GetCandidates(vocabSize, logits);


                _constraints.ApplyConstraints(candidatesP, outputTokens, logits);
                var id = Sample(candidatesP);

                if (id == _eosToken)
                {
                    Console.WriteLine("EOS");
                    break;
                }

                outputTokens.Add(id);
            }


            return outputTokens.Select(TokenToString).ToList();
        }

        private int GetVocab(out Span<float> logits, Dictionary<int, float>? logitBias = null)
        {
            var vocabSize = _contextHandle.llama_n_vocab();
            logits = GetLogits(_contextHandle, vocabSize);

            if (logitBias is null)
            {
                return vocabSize;
            }

            foreach (var (key, value) in logitBias)
            {
                logits[key] += value;
            }

            return vocabSize;
        }

        private static TokenDataArray GetCandidates(int vocabSize, Span<float> logits)
        {
            var candidates = new List<TokenData> { Capacity = vocabSize };
            for (var tokenId = 0; tokenId < vocabSize; tokenId++)
            {
                candidates.Add(new TokenData(tokenId, logits[tokenId], 0.0f));
            }

            var candidatesP = new TokenDataArray(candidates.ToArray(), (ulong)candidates.Count, false);
            return candidatesP;
        }

        private void TryEvaluate(InferenceOptions options, int[] tokens, out bool result)
        {
            var evalResult = _contextHandle.llama_eval(tokens, tokens.Length, tokens.Length,
                options.Threads);
            result = evalResult == 0;
        }

        private int Sample(TokenDataArray st)
        {
            _contextHandle.llama_sample_temperature(st, 1.0f);

            var id = _contextHandle.llama_sample_token(st);
            return id;
        }


        private static unsafe Span<float> GetLogits(SafeLLamaContextHandle contextHandle, int length)
        {
            var logits = contextHandle.llama_get_logits();
            return new Span<float>(logits, length);
        }


        private static void ProgressCallback(float progress, IntPtr ctx)
        {
            Console.WriteLine($"Progress: {progress}");
        }

        private static void InitializeLora(SafeLLamaContextHandle contextHandle, string modelPath,
            string loraAdapterPath, int optionsLoraThreads)
        {
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
}
