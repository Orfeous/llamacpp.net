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

    private readonly int[] _lastTokens;
    private readonly ulong _lastTokensSize;

    public FrequencyAndPresencePenaltySampler(float alphaFrequency, float alphaPresence)
    {
        _lastTokens = Array.Empty<int>();
        _lastTokensSize = 0;
        _alphaFrequency = alphaFrequency;
        _alphaPresence = alphaPresence;
    }


    public void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_frequency_and_presence_penalties(intPtr, _lastTokens, _lastTokensSize,
            _alphaFrequency, _alphaPresence);
    }
}