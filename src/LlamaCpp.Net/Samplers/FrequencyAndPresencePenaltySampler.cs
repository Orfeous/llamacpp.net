using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

internal sealed class FrequencyAndPresencePenaltySampler : ISampler
{
    /// <summary>
    ///     The frequency penalty coefficient
    /// </summary>
    private readonly float _alphaFrequency;

    /// <summary>
    ///     The presence penalty coefficient
    /// </summary>
    private readonly float _alphaPresence;


    public FrequencyAndPresencePenaltySampler(float alphaFrequency, float alphaPresence)
    {
        _alphaFrequency = alphaFrequency;
        _alphaPresence = alphaPresence;
    }


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr, int[] currentOutput)
    {

        context.llama_sample_frequency_and_presence_penalties(intPtr, currentOutput, (ulong)currentOutput.Length,
            _alphaFrequency, _alphaPresence);
    }
}