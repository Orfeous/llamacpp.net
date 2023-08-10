using LlamaCpp.Net.Native.Abstractions;
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


    public void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput)
    {
        context.SampleTopK(intPtr, _k, _minKeep);
    }
}