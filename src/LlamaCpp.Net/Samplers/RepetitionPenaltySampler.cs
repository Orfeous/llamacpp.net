using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Apply repetition penalty to the candidates
///     Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
/// </summary>
internal sealed class RepetitionPenaltySampler : AbstractSampler
{
    private readonly int[] _lastTokens;
    private readonly ulong _lastTokensSize;
    private readonly float _penalty;


    public RepetitionPenaltySampler(SafeLLamaContextHandle context, int[] lastTokens, InferenceOptions inferenceOptions) : base(context)
    {
        _lastTokens = lastTokens;
        _lastTokensSize = inferenceOptions.RepetitionLastN;
        _penalty = inferenceOptions.RepetitionPenalty;
    }


    public override void Sample(IntPtr intPtr)
    {
        _context.llama_sample_repetition_penalty(intPtr, _lastTokens, _lastTokensSize, _penalty);
    }
}