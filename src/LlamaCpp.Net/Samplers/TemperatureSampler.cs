using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class TemperatureSampler : AbstractSampler
{
    private readonly float _temperature;

    public TemperatureSampler(SafeLLamaContextHandle context, InferenceOptions options) : base(context)
    {
        _temperature = options.Temperature;
    }


    public override void Sample(IntPtr intPtr)
    {
        _context.llama_sample_temperature(intPtr, _temperature);
    }
}