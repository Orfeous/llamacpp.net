using System.Collections.Generic;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using Microsoft.Extensions.Logging;

namespace LlamaCpp.Net.Samplers.Pipelines;

/// <summary>
///     Builds a <see cref="SamplingPipeline" /> instance.
/// </summary>
internal sealed class SamplingPipelineBuilder : ISamplingPipelineBuilder
{
    private readonly ILogger _logger;
    private readonly IList<ISampler> _samplers = new List<ISampler>();

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    public SamplingPipelineBuilder(ILogger logger)
    {
        _logger = logger;
    }


    public ISamplingPipelineBuilder AddTemperatureSampler(float temperature)
    {
        _samplers.Add(new TemperatureSampler(temperature));
        return this;
    }


    public ISamplingPipelineBuilder AddSoftMaxSampler()
    {
        _samplers.Add(new SoftMaxSampler());
        return this;
    }

    public ISamplingPipelineBuilder AddTailFreeSampler(int tailFreeZ, ulong minKeep)
    {
        _samplers.Add(new TailFreeSampler(tailFreeZ, minKeep));
        return this;
    }

    public ISamplingPipelineBuilder AddTopPSampler(float topP, ulong minKeep)
    {
        _samplers.Add(new TopPSampler(topP, minKeep));

        return this;
    }

    public ISamplingPipelineBuilder AddTopKSampler(int topK, ulong minKeep)
    {
        _samplers.Add(new TopKSampler(topK, minKeep));

        return this;
    }

    public ISamplingPipelineBuilder AddTypicalSampler(int localTypicalK, ulong minKeep)
    {
        _samplers.Add(new TypicalSampler(localTypicalK, minKeep));

        return this;
    }

    public ISamplingPipelineBuilder AddRepetitionPenaltySampler(ulong repetitionLastN, float penalty)
    {
        _samplers.Add(new RepetitionPenaltySampler(repetitionLastN, penalty));

        return this;
    }

    public ISamplingPipelineBuilder AddFrequencyAndPresencePenaltySampler(float alphaFrequency, float alphaPresence)
    {
        _samplers.Add(new FrequencyAndPresencePenaltySampler(alphaFrequency, alphaPresence));

        return this;
    }


    internal SamplingPipeline Build(SafeLLamaContextHandle contextHandle)
    {
        return new SamplingPipeline(contextHandle, _samplers);
    }
}