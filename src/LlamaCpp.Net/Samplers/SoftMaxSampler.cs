using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;


internal sealed class SoftMaxSampler : ISampler
{
    internal SoftMaxSampler()
    {
    }

    public void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput)
    {
        context.SampleSoftmax(intPtr);
    }
}