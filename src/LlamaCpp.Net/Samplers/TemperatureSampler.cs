using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TemperatureSampler : AbstractSampler
{
    private readonly float _temperature;

    private TemperatureSampler(SafeLLamaContextHandle context, float temperature) : base(context)
    {
        _temperature = temperature;
    }

    public static TemperatureSampler CreateInstance(SafeLLamaContextHandle context, float temperature)
    {
        return new TemperatureSampler(context, temperature);
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_temperature(intPtr, _temperature);
    }
}