using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TypicalSampler : ISampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    internal TypicalSampler(int localTypicalK, ulong minKeep)
    {
        _k = localTypicalK;
        _minKeep = minKeep;

    }

    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_typical(intPtr, _k, _minKeep);
    }
}