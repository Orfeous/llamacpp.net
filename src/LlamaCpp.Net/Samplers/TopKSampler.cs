using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
/// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
/// </summary>
internal sealed class TopKSampler : AbstractSampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    public TopKSampler(ref SafeLLamaContextHandle context, int k, ulong minKeep) : base(context)
    {
        _k = k;
        _minKeep = minKeep;
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
        => context.llama_sample_top_k(intPtr, _k, _minKeep);
}