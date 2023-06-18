using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Native;

namespace LlamaCpp.Net
{
    /// <inheritdoc />
    public class LanguageModel : IDisposable
    {
        private SafeLLamaContextHandle? contextHandle;

        string ModelPath { get; }

        private LanguageModelOptions Options { get; init; }

        /// <summary>
        /// The constructor for the language model
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="options"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public LanguageModel(string modelPath, LanguageModelOptions? options = null)
        {
            ModelPath = modelPath;


            if (File.Exists(ModelPath) == false)
            {
                throw new FileNotFoundException($"Model file {modelPath} not found", modelPath);
            }

            if (options == null)
            {
                Options = LanguageModelOptions.Default;
            }
            else
            {
                Options = options;

                if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
                {
                    if (File.Exists(options.LoraAdapterPath) == false)
                    {
                        throw new FileNotFoundException($"Lora adapter file {options.LoraAdapterPath} not found",
                            options.LoraAdapterPath);
                    }
                }
            }


            contextHandle = InitializeContext(ModelPath, Options);
        }

        private static SafeLLamaContextHandle InitializeContext(string modelPath, LanguageModelOptions options)
        {
            var contextParams = LlamaNative.llama_context_default_params();
            options.Apply(contextParams);

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

            var contextHandle = new SafeLLamaContextHandle(context);

            if (!string.IsNullOrWhiteSpace(options.LoraAdapterPath))
            {
                InitializeLora(contextHandle, modelPath, options.LoraAdapterPath, options.LoraThreads);
            }

            return contextHandle;
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


        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the context handle
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                contextHandle?.Dispose();
            }
        }
    }
}
