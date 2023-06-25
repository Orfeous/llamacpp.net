using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TemperatureSampler : ISampler
{
    private readonly float _temperature;

    public TemperatureSampler(float temperature)
    {
        _temperature = temperature;
    }


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr, int[] currentOutput)
    {
        context.llama_sample_temperature(intPtr, _temperature);
    }
}