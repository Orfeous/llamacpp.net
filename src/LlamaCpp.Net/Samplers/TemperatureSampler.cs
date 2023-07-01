using LlamaCpp.Net.Native.Abstractions;
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


    public void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput)
    {
        context.SampleTemperature(intPtr, _temperature);
    }
}