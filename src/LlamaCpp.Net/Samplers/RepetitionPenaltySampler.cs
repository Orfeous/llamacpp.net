using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Apply repetition penalty to the candidates
///     Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
/// </summary>
internal sealed class RepetitionPenaltySampler : ISampler
{
    private readonly int[] _lastTokens;
    private readonly ulong _lastTokensSize;
    private readonly float _penalty;


    public RepetitionPenaltySampler(ulong repetitionLastN, float repetitionPenalty)
    {
        _lastTokens = Array.Empty<int>();
        _lastTokensSize = repetitionLastN;
        _penalty = repetitionPenalty;
    }


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_repetition_penalty(intPtr, _lastTokens, _lastTokensSize, _penalty);
    }
}