using System;
using System.Collections.Generic;
using System.Linq;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers.Abstractions;

namespace LlamaCpp.Net.Samplers.Pipelines;

/// <inheritdoc />
internal sealed unsafe class SamplingPipeline : ISamplingPipeline
{
    private readonly ILlamaInstance _contextHandle;
    private readonly int _newLineToken;
    private readonly IList<ISampler> _samplers;
    private readonly SamplingMethod _endSampler;


    public SamplingPipeline(ILlamaInstance contextHandle, IList<ISampler> samplers, SamplingMethod endSampler)
    {
        _contextHandle = contextHandle;
        _samplers = samplers;
        _endSampler = endSampler;
        _newLineToken = LlamaNative.llama_token_nl();
    }


    /// <inheritdoc />
    public int ApplyConstraints(TokenDataArray candidatesP,
        Span<float> logits,
        int[] currentOutput,
        bool penalizeNewLine = false,
        int repetitionLookback = 0
        )
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

        // create a subset of currentOutput that only contains the last n tokens
        // this is used to apply the frequency penalty
        var n = repetitionLookback;

        var currentOutputSet = currentOutput.Length > n
            ? currentOutput[^n..]
            : currentOutput;



        if (_samplers.Any())
        {
            foreach (var sampler in _samplers)
            {
                sampler.Sample(_contextHandle, ptr, currentOutputSet);
            }
        }


        var newLineLogit = logits[_newLineToken];

        if (!penalizeNewLine)
        {
            logits[_newLineToken] = newLineLogit;
        }

        var mu = 0.0f;

        return _endSampler switch
        {
            SamplingMethod.Mirostat => _contextHandle.SampleTokenMirostat(ptr, 1, 1, 100, &mu),
            SamplingMethod.MirostatV2 => _contextHandle.SampleTokenMirostatV2(ptr, 1, 1, &mu),
            SamplingMethod.Default => _contextHandle.SampleToken(ptr),
            _ => throw new ArgumentOutOfRangeException(nameof(candidatesP))
        };
    }
}