using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TopPSampler : ISampler
{
    private readonly ulong _minKeep;
    private readonly float _p;

    public TopPSampler(float topP, ulong minKeep)
    {
        _minKeep = minKeep;
        _p = topP;
    }


    public void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput)
    {
        context.SampleTopP(intPtr, _p, _minKeep);
    }
}