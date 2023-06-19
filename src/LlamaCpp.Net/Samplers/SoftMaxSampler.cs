using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
/// </summary>
internal sealed class SoftMaxSampler : AbstractSampler
{
    private SoftMaxSampler(SafeLLamaContextHandle context) : base(context)
    {
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_softmax(intPtr);
    }

    public static SoftMaxSampler CreateInstance(SafeLLamaContextHandle context)
    {
        return new SoftMaxSampler(context);
    }
}