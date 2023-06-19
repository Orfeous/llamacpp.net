using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
/// </summary>
internal sealed class TailFreeSampler : AbstractSampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    private TailFreeSampler(ref SafeLLamaContextHandle context, int k, ulong minKeep) : base(context)
    {
        _k = k;
        _minKeep = minKeep;
    }

    public static TailFreeSampler CreateInstance(ref SafeLLamaContextHandle context, int k, ulong minKeep)
    {
        return new TailFreeSampler(ref context, k, minKeep);
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_tail_free(intPtr, _k, _minKeep);
    }
}