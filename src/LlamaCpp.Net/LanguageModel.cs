using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        private readonly Encoding _encoding = Encoding.UTF8;
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
