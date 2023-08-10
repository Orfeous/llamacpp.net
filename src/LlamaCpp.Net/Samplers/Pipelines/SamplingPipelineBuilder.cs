using System.Collections.Generic;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native.Abstractions;
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
    private SamplingMethod _endSampler;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    public SamplingPipelineBuilder(ILogger logger)
    {
        _logger = logger;
    }


    public ISamplingPipelineBuilder AddTemperatureSampler(float temperature)
    {
        _logger.LogInformation($"Adding temperature sampler with temperature={temperature}");
        _samplers.Add(new TemperatureSampler(temperature));
        return this;
    }


    public ISamplingPipelineBuilder AddSoftMaxSampler()
    {
        _logger.LogInformation("Adding softmax sampler");
        _samplers.Add(new SoftMaxSampler());
        return this;
    }

    public ISamplingPipelineBuilder AddTailFreeSampler(int tailFreeZ, ulong minKeep)
    {
        _logger.LogInformation($"Adding tail free sampler with z={tailFreeZ} and minKeep={minKeep}");
        _samplers.Add(new TailFreeSampler(tailFreeZ, minKeep));
        return this;
    }

    public ISamplingPipelineBuilder AddTopPSampler(float topP, ulong minKeep)
    {
        _logger.LogInformation($"Adding topP sampler with topP={topP} and minKeep={minKeep}");
        _samplers.Add(new TopPSampler(topP, minKeep));

        return this;
    }

    public ISamplingPipelineBuilder AddTopKSampler(int topK, ulong minKeep)
    {
        _logger.LogInformation($"Adding topK sampler with topK={topK} and minKeep={minKeep}");
        _samplers.Add(new TopKSampler(topK, minKeep));

        return this;
    }

    public ISamplingPipelineBuilder AddTypicalSampler(int localTypicalK, ulong minKeep)
    {
        _logger.LogInformation($"Adding typical sampler with localTypicalK={localTypicalK} and minKeep={minKeep}");
        _samplers.Add(new TypicalSampler(localTypicalK, minKeep));

        return this;
    }

    public ISamplingPipelineBuilder AddRepetitionPenaltySampler(float penalty)
    {
        _logger.LogInformation($"Adding repetition penalty sampler with penalty={penalty}");
        _samplers.Add(new RepetitionPenaltySampler(penalty));

        return this;
    }

    public ISamplingPipelineBuilder AddFrequencyAndPresencePenaltySampler(float alphaFrequency, float alphaPresence)
    {
        _logger.LogInformation($"Adding frequency and presence penalty sampler with alphaFrequency={alphaFrequency} and alphaPresence={alphaPresence}");
        _samplers.Add(new FrequencyAndPresencePenaltySampler(alphaFrequency, alphaPresence));

        return this;
    }




    public SamplingPipeline Build(ILlamaInstance contextHandle)
    {
        return new SamplingPipeline(contextHandle, _samplers, _endSampler);
    }

    public ISamplingPipelineBuilder SetEndSampler(SamplingMethod endSampler)
    {
        _logger.LogInformation($"Setting end sampler to {endSampler}");
        this._endSampler = endSampler;
        return this;
    }
}