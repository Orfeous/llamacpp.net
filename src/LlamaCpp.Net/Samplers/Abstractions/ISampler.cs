using LlamaCpp.Net.Native.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers.Abstractions;

/// <summary>
///     A wrapper for c++ sampler methods.
/// </summary>
internal interface ISampler
{
    protected internal void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput);
}