using System;
using System.Collections.Generic;
using System.Threading;
using LlamaCpp.Net.Configuration;

namespace LlamaCpp.Net.Abstractions
{
    /// <summary>
    /// An interface for the language model
    /// </summary>
    public interface ILanguageModel : IDisposable
    {
        /// <summary>
        /// The path to the model
        /// </summary>
        string ModelPath { get; init; }

        /// <summary>
        /// The options for the language model
        /// </summary>
        LanguageModelOptions Options { get; init; }


        /// <summary>
        /// Tokenizes the input
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        List<int> Tokenize(string text);

        /// <summary>
        /// Convert a token to a string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string TokenToString(int token);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        IEnumerable<string> InferAsync(string input, InferenceOptions? options = null, CancellationToken cancellationToken = default);
    }
}
