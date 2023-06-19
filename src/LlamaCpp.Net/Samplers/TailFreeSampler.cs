using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TailFreeSampler : AbstractSampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    public TailFreeSampler(ref SafeLLamaContextHandle context, int k, ulong minKeep) : base(context)
    {
        _k = k;
        _minKeep = minKeep;
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
        => context.llama_sample_tail_free(intPtr, _k, _minKeep);
}