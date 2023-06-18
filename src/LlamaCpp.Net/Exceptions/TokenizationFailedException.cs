using System;

namespace LlamaCpp.Net.Exceptions
{
    /// <summary>
    /// Exception thrown when tokenization fails
    /// </summary>
    public class TokenizationFailedException : Exception
    {
        /// <summary>
        ///    Constructor
        /// </summary>
        /// <param name="text"></param>
        public TokenizationFailedException(string text) : base($"Tokenization of {text} failed")
        {
        }
    }
}
