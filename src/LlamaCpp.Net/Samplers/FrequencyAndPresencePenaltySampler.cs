using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///    Apply frequency and presence penalties to the candidates
///    described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
/// </summary>
internal sealed class FrequencyAndPresencePenaltySampler : AbstractSampler
{
    private readonly int[] _lastTokens;
    private readonly ulong _lastTokensSize;

    /// <summary>
    /// The frequency penalty coefficient
    /// </summary>
    private readonly float _alphaFrequency;

    /// <summary>
    /// The presence penalty coefficient
    /// </summary>
    private readonly float _alphaPresence;

    public FrequencyAndPresencePenaltySampler(SafeLLamaContextHandle context, int[] lastTokens,
        ulong lastTokensSize, float alphaFrequency, float alphaPresence) : base(context)
    {
        _lastTokens = lastTokens;
        _lastTokensSize = lastTokensSize;
        _alphaFrequency = alphaFrequency;
        _alphaPresence = alphaPresence;
    }

    protected override void Sample(SafeLLamaContextHandle context, IntPtr intPtr)
        => context.llama_sample_frequency_and_presence_penalties(intPtr, _lastTokens, _lastTokensSize,
            _alphaFrequency, _alphaPresence);
}