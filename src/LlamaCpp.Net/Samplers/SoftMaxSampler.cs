using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;


internal sealed class SoftMaxSampler : ISampler
{
    internal SoftMaxSampler()
    {
    }

    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_softmax(intPtr);
    }
}