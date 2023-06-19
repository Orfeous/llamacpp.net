using LlamaCpp.Net.Native.Models;

namespace LlamaCpp.Net.Samplers.Abstractions;

/// <summary>
///     A wrapper for c++ sampler methods.
/// </summary>
internal interface ISampler
{
    protected internal void Sample(TokenDataArray candidates);
}