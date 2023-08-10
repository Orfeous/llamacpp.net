using System;

namespace LlamaCpp.Net.Exceptions
{
    /// <summary>
    /// An exception to throw when the lora adapter fails to initialize
    /// </summary>
    public sealed class LoraAdapterFailedInitializationException : Exception
    {
        /// <summary>
        /// Constructor for the exception
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="loraAdapterPath"></param>
        /// <param name="loraThreads"></param>
        internal LoraAdapterFailedInitializationException(string modelPath, string loraAdapterPath, int loraThreads) :
            base(
                $"Failed to initialize lora adapter {loraAdapterPath} for model {modelPath} with {loraThreads} threads")
        {
            ModelPath = modelPath;
            LoraAdapterPath = loraAdapterPath;
            LoraThreads = loraThreads;
        }

        /// <summary>
        /// The path to the model
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// The path to the lora adapter
        /// </summary>
        public string LoraAdapterPath { get; set; }

        /// <summary>
        /// The number of threads to use
        /// </summary>

        public int LoraThreads { get; set; }
    }
}
