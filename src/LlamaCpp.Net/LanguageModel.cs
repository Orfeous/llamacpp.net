using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly SafeLLamaContextHandle _contextHandle;
        private readonly int _contextSize;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly int _eosToken;
        private readonly ILogger<LanguageModel> _logger;
        private readonly int _nlToken;

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
            _nlToken = LlamaNative.llama_token_nl();
            _contextSize = LlamaNative.llama_n_ctx(_contextHandle);
        }


        /// <inheritdoc />

        public string ModelPath { get; init; }

        /// <inheritdoc />

        public LanguageModelOptions Options { get; init; }

        /// <inheritdoc />
        public List<int> Tokenize(string text)
        {
            var tokens = new int[text.Length + 1];


            var numberOfTokens = LlamaNative.llama_tokenize(_contextHandle, text, tokens, tokens.Length, true);

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
            var ptr = LlamaNative.llama_token_to_str(_contextHandle, token);
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
                var evalResult = LlamaNative.llama_eval(_contextHandle, tokens, tokens.Length, tokens.Length,
                    options.Threads);

                if (evalResult != 0)
                {
                    break;
                }

                var candidates = ApplyPenalty(outputTokens);


                var id = Sample(candidates);

                if (id == _eosToken)
                {
                    Console.WriteLine("EOS");
                    break;
                }

                Debug.WriteLine($"{id} : {TokenToString(id)}");
                outputTokens.Add(id);
            }


            return outputTokens.Select(TokenToString).ToList();
        }

        private int Sample(TokenDataArray st)
        {
            llama_sample_temperature(_contextHandle, st, 1.0f);


            var id = llama_sample_token(_contextHandle, st);
            return id;
        }

        private static unsafe int llama_sample_token(SafeLLamaContextHandle ctx, TokenDataArray candidates)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer), size = candidates.size, sorted = candidates.sorted
            };
            return LlamaNative.llama_sample_token(ctx, new IntPtr(&st));
        }

        private static unsafe void llama_sample_temperature(SafeLLamaContextHandle ctx, TokenDataArray candidates,
            float temp)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer), size = candidates.size, sorted = candidates.sorted
            };
            LlamaNative.llama_sample_temperature(ctx, new IntPtr(&st), temp);
        }


        private TokenDataArray ApplyPenalty(IEnumerable<int> lastTokens,
            Dictionary<int, float>? logitBias = null,
            int repeatLastTokensCount = 64, float repeatPenalty = 1.1f, float alphaFrequency = .0f,
            float alphaPresence = .0f,
            bool penalizeNl = true)
        {
            var vocabSize = LlamaNative.llama_n_vocab(_contextHandle);
            var logits = GetLogits(_contextHandle, vocabSize);

            if (logitBias is not null)
            {
                foreach (var (key, value) in logitBias)
                {
                    logits[key] += value;
                }
            }

            var candidates = new List<TokenData> { Capacity = vocabSize };
            for (var tokenId = 0; tokenId < vocabSize; tokenId++)
            {
                candidates.Add(new TokenData(tokenId, logits[tokenId], 0.0f));
            }

            var candidatesP = new TokenDataArray(candidates.ToArray(), (ulong)candidates.Count, false);

            // Apply penalties
            var nlLogit = logits[_nlToken];

            var lt = lastTokens.ToList();
            var lastTokensCount = lt.Count;
            // TODO: set context size
            var lastNRepeat = Math.Min(Math.Min(lastTokensCount, repeatLastTokensCount), _contextSize);


            SampleRepetitionPenalty(_contextHandle, candidatesP,
                lt.Skip(lastTokensCount - lastNRepeat).ToArray(),
                (ulong)lastNRepeat, repeatPenalty);
            SampleFrequencyAndPresencePenalties(_contextHandle, candidatesP,
                lt.Skip(lastTokensCount - lastNRepeat).ToArray(),
                (ulong)lastNRepeat, alphaFrequency, alphaPresence);


            if (!penalizeNl)
            {
                logits[_nlToken] = nlLogit;
            }

            return candidatesP;
        }

        private static unsafe void SampleRepetitionPenalty(SafeLLamaContextHandle ctx,
            TokenDataArray candidates,
            int[] lastTokens, ulong lastTokensSize, float penalty)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer), size = candidates.size, sorted = candidates.sorted
            };
            LlamaNative.llama_sample_repetition_penalty(ctx, new IntPtr(&st), lastTokens, lastTokensSize, penalty);
        }

        private static unsafe void SampleFrequencyAndPresencePenalties(SafeLLamaContextHandle ctx,
            TokenDataArray candidates, int[] lastTokens, ulong lastTokensSize, float alphaFrequency,
            float alphaPresence)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer), size = candidates.size, sorted = candidates.sorted
            };
            LlamaNative.llama_sample_frequency_and_presence_penalties(ctx, new IntPtr(&st), lastTokens,
                lastTokensSize,
                alphaFrequency, alphaPresence);
        }


        private static unsafe Span<float> GetLogits(SafeLLamaContextHandle contextHandle, int length)
        {
            var logits = LlamaNative.llama_get_logits(contextHandle);
            return new Span<float>(logits, length);
        }


        private static void ProgressCallback(float progress, IntPtr ctx)
        {
            Console.WriteLine($"Progress: {progress}");
        }

        private static void InitializeLora(SafeLLamaContextHandle contextHandle, string modelPath,
            string loraAdapterPath, int optionsLoraThreads)
        {
            var r = LlamaNative.llama_apply_lora_from_file(contextHandle, loraAdapterPath, modelPath,
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
                _contextHandle?.Dispose();
            }
        }
    }
}
