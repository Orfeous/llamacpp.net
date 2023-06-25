using System;

namespace LlamaCpp.Net.Exceptions;

/// <summary>
/// </summary>
public class QuantizeException : Exception
{
    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="s"></param>
    public QuantizeException(int s) : base("Quantization failed with error code " + s)
    {
    }
}