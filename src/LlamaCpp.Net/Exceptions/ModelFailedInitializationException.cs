using System;

namespace LlamaCpp.Net.Exceptions
{
    /// <summary>
    /// Simple exception to throw when the model fails to initialize
    /// </summary>
    public sealed class ModelFailedInitializationException : Exception
    {
        /// <summary>
        /// Constructor for the exception
        /// </summary>
        /// <param name="modelPath"></param>
        internal ModelFailedInitializationException(string modelPath) : base(
            $"Failed to initialize model {modelPath}")
        {
            ModelPath = modelPath;
        }

        /// <summary>
        /// The path to the model
        /// </summary>
        public string ModelPath { get; set; }
    }
}