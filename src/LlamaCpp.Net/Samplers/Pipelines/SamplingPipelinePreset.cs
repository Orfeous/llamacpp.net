using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers.Pipelines;

/// <summary>
///     A collection of sampling pipeline presets.
/// </summary>
public class SamplingPipelinePreset
{
    /// <summary>
    ///     A sampling pipeline that uses the default sampling method and temperature.
    /// </summary>
    public static Action<ISamplingPipelineBuilder> Default =>
        builder => builder.AddTemperatureSampler(1);

    /// <summary>
    ///     Inference options for having a conversation with the model
    /// </summary>
    public static Action<ISamplingPipelineBuilder> Chat =>
        builder => builder.AddTemperatureSampler(0.8f)
            .AddRepetitionPenaltySampler(1.1f)
            .AddFrequencyAndPresencePenaltySampler(0.8f, 0.3f)
            .AddTopPSampler(0.7f, 200)
            .AddTopKSampler(0, 200);

    /// <summary>
    ///     Inference options for getting precise results from the model
    /// </summary>
    public static Action<ISamplingPipelineBuilder> Precise =>
        builder => builder.AddTemperatureSampler(0.7f)
            .AddRepetitionPenaltySampler(1.1f)
            .AddTopPSampler(0.1f, 200)
            .AddTopKSampler(40, 200);
}