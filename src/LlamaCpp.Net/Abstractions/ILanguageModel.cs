using LlamaCpp.Net.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LlamaCpp.Net.Abstractions;

/// <summary>
///     An interface for the language model
/// </summary>
public interface ILanguageModel : IDisposable
{
    /// <summary>
    ///     The path to the model
    /// </summary>
    string ModelPath { get; init; }

    /// <summary>
    ///     The options for the language model
    /// </summary>
    LanguageModelOptions Options { get; init; }


    /// <summary>
    ///     Tokenizes the input
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    List<int> Tokenize(string text);


    /// <summary>
    ///     Converts a token to its corresponding string representation
    /// </summary>
    /// <param name="token">The token to convert</param>
    /// <returns>The string representation of the token</returns>
    string TokenToString(int token);


    /// <summary>
    ///     Infers the output for the given input
    /// </summary>
    /// <param name="input">The input to infer the output for</param>
    /// <param name="options">The options for the inference</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The inferred output</returns>
    IAsyncEnumerable<string> InferAsync(string input, InferenceOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    IEnumerable<string> Infer(string input, InferenceOptions? options = null);

}