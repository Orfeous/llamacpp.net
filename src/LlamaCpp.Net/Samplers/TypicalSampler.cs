using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
/// </summary>
internal sealed class TypicalSampler : AbstractSampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    private TypicalSampler(SafeLLamaContextHandle context, int k, ulong minKeep) : base(context)
    {
        _k = k;
        _minKeep = minKeep;
    }

    public static TypicalSampler CreateInstance(SafeLLamaContextHandle context, int k, ulong minKeep)
    {
        return new TypicalSampler(context, k, minKeep);
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_typical(intPtr, _k, _minKeep);
    }
}