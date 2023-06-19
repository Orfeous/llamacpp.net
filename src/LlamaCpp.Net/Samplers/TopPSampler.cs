using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
/// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
/// </summary>
internal sealed class TopPSampler : AbstractSampler
{
    private readonly float _p;
    private readonly ulong _minKeep;

    public TopPSampler(ref SafeLLamaContextHandle context, float p, ulong minKeep) : base(context)
    {
        _p = p;
        _minKeep = minKeep;
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
        => context.llama_sample_top_p(intPtr, _p, _minKeep);
}