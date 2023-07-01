using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;


internal sealed class TailFreeSampler : ISampler
{
    private readonly int _z;
    private readonly ulong _minKeep;

    internal TailFreeSampler(int tailFreeZ, ulong minKeep)
    {
        _z = tailFreeZ;
        _minKeep = minKeep;
    }


    public void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput)
    {
        context.SampleTailFree(intPtr, _z, _minKeep);
    }
}