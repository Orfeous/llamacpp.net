using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
/// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
/// </summary>
internal sealed class SoftMaxSampler : AbstractSampler
{
    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
        => context.llama_sample_softmax(intPtr);


    public SoftMaxSampler(SafeLLamaContextHandle context) : base(context)
    {
    }
}