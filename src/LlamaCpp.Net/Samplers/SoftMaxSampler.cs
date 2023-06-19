using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class SoftMaxSampler : AbstractSampler
{
    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
        => context.llama_sample_softmax(intPtr);


    public SoftMaxSampler(SafeLLamaContextHandle context) : base(context)
    {
    }
}