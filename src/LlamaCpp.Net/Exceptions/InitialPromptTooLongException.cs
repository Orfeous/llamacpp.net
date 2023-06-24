using System;

namespace LlamaCpp.Net.Exceptions;


/// <summary>
/// The exception that is thrown when the initial prompt for a conversation is too long.
/// </summary>
public class InitialPromptTooLongException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitialPromptTooLongException"/> class.
    /// </summary>
    /// <param name="contextSize"></param>
    public InitialPromptTooLongException(int contextSize) : base(
        $"The initial prompt is too long. It must be less than {contextSize} tokens.")
    {
    }
}