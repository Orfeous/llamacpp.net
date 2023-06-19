using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Apply frequency and presence penalties to the candidates
///     described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
/// </summary>
internal sealed class FrequencyAndPresencePenaltySampler : AbstractSampler
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

    private FrequencyAndPresencePenaltySampler(SafeLLamaContextHandle context, int[] lastTokens,
        ulong lastTokensSize, float alphaFrequency, float alphaPresence) : base(context)
    {
        _lastTokens = lastTokens;
        _lastTokensSize = lastTokensSize;
        _alphaFrequency = alphaFrequency;
        _alphaPresence = alphaPresence;
    }

    public static FrequencyAndPresencePenaltySampler CreateInstance(SafeLLamaContextHandle context, int[] lastTokens,
        ulong lastTokensSize, float alphaFrequency, float alphaPresence)
    {
        return new FrequencyAndPresencePenaltySampler(context, lastTokens, lastTokensSize, alphaFrequency,
            alphaPresence);
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
    {
        context.llama_sample_frequency_and_presence_penalties(intPtr, _lastTokens, _lastTokensSize,
            _alphaFrequency, _alphaPresence);
    }
}