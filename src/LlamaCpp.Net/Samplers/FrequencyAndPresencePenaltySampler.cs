using System;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Samplers.Abstractions;

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


    public void Sample(ILlamaInstance context, IntPtr intPtr, int[] currentOutput)
    {
        context.SampleFrequencyAndPresencePenalties(intPtr, currentOutput, (ulong)currentOutput.Length,
            _alphaFrequency, _alphaPresence);
    }
}