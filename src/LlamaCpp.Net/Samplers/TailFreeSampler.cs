using LlamaCpp.Net.Native;
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


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_tail_free(intPtr, _z, _minKeep);
    }
}