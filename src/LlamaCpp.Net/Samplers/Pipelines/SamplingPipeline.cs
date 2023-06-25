using System;
using System.Collections.Generic;
using System.Linq;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers.Abstractions;

namespace LlamaCpp.Net.Samplers.Pipelines;

/// <summary>
///     Represents a set of constraints that can be applied to token candidates based on the last tokens and inference
///     options.
/// </summary>
internal sealed unsafe class SamplingPipeline
{
    private readonly SafeLLamaContextHandle _contextHandle;
    private readonly int _newLineToken;
    private readonly IList<ISampler> _samplers;


    public SamplingPipeline(SafeLLamaContextHandle contextHandle, IList<ISampler> samplers)
    {
        _contextHandle = contextHandle;
        _samplers = samplers;
        _newLineToken = LlamaNative.llama_token_nl();
    }


    /// <summary>
    ///     Applies constraints to the given token candidates based on the last tokens and inference options.
    /// </summary>
    /// <param name="candidatesP">The token candidates to apply constraints to.</param>
    /// <param name="logits">The logits for each token.</param>
    /// <param name="inferenceOptions">The inference options to use for applying constraints.</param>
    public int ApplyConstraints(TokenDataArray candidatesP,
        Span<float> logits,
        InferenceOptions inferenceOptions)
    {
        // Pin the data array to prevent the garbage collector from moving it around
        var handle = candidatesP.data.Pin();

        // Create a native representation of the token data array
        var st = new TokenDataArrayNative
        {
            data = new IntPtr(handle.Pointer),
            size = candidatesP.size,
            sorted = candidatesP.sorted
        };
        var ptr = new IntPtr(&st);


        if (_samplers.Any())
        {
            foreach (var sampler in _samplers)
            {
                sampler.Sample(_contextHandle, ptr);
            }
        }


        var newLineLogit = logits[_newLineToken];

        if (!inferenceOptions.PenalizeNewLine)
        {
            logits[_newLineToken] = newLineLogit;
        }

        var mu = 0.0f;

        return inferenceOptions.SamplingMethod switch
        {
            SamplingMethod.Mirostat => _contextHandle.llama_sample_token_mirostat(ptr, 1, 1, 100, &mu),
            SamplingMethod.MirostatV2 => _contextHandle.llama_sample_token_mirostat_v2(ptr, 1, 1, &mu),
            SamplingMethod.Default => _contextHandle.llama_sample_token(ptr),
            _ => throw new ArgumentOutOfRangeException(nameof(candidatesP))
        };
    }
}