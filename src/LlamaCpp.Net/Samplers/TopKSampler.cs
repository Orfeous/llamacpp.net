using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TopKSampler : ISampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    public TopKSampler(int topK, ulong minKeep)
    {
        _k = topK;
        _minKeep = minKeep;
    }


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr, int[] currentOutput)
    {
        context.llama_sample_top_k(intPtr, _k, _minKeep);
    }
}