using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
///     Locally typical sampling promotes the generation of contextually coherent and diverse text
///     by sampling tokens that are typical or expected based on the surrounding context.
///     By setting the parameter p between 0 and 1, you can control the balance between producing text that is locally
///     coherent and diverse.
///     A value closer to 1 will promote more contextually coherent tokens,
///     while a value closer to 0 will promote more diverse tokens. A value equal to 1 disables locally typical sampling.
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