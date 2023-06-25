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
    private readonly float _penalty;


    public RepetitionPenaltySampler(float repetitionPenalty)
    {
        _penalty = repetitionPenalty;
    }


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr, int[] currentOutput)
    {

        context.llama_sample_repetition_penalty(intPtr, currentOutput, (ulong)currentOutput.Length, _penalty);
    }
}