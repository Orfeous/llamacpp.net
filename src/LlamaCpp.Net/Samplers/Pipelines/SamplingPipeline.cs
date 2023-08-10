using System;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Native.Models;
using Microsoft.Extensions.Logging;

namespace LlamaCpp.Net.Samplers.Pipelines;

/// <inheritdoc />
internal sealed unsafe class SamplingPipeline
{
    private readonly ILlamaInstance _contextHandle;
    private readonly ILogger _logger;
    private readonly int _newLineToken;


    public SamplingPipeline(ILlamaInstance contextHandle, ILogger logger)
    {
        _contextHandle = contextHandle;
        _logger = logger;
        _newLineToken = LlamaNative.llama_token_nl();
    }


    /// <inheritdoc />
    public int ApplyConstraints(TokenDataArray candidatesP,
        Span<float> logits,
        int[] currentOutput,
        InferenceOptions options
    )
    {
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
        var n = options.RepetitionLookback;

        // get the smallest value of n, the length of the current output, and the context size
        var lastNRepeat = (ulong)Math.Min(Math.Min(n, currentOutput.Length), _contextHandle.GetContextSize());


        // apply sample repetition penalty


        _contextHandle.SampleRepetitionPenalty(ptr, currentOutput, lastNRepeat, options.RepeatPenalty);

        // apply frequency penalty

        _contextHandle.SampleFrequencyAndPresencePenalties(ptr, currentOutput, lastNRepeat, options.FrequencyPenalty,
            options.PresencePenalty);

        var newLineLogit = logits[_newLineToken];


        var endSampler = options.SamplingMethod;

        if (options.Temperature <= 0 || endSampler == SamplingMethod.Greedy)
        {
            // Greedy sampling
            return SampleTokenGreedy(ptr);
        }


        if (endSampler == SamplingMethod.Mirostat)
        {
            _contextHandle.SampleTemperature(ptr, options.Temperature);
        }
        else if (options.SamplingMethod == SamplingMethod.MirostatV2)
        {
            _contextHandle.SampleTemperature(ptr, options.Temperature);
        }
        else
        {
            // Temperature sampling
            _contextHandle.SampleTopK(ptr, options.TopK, 1);
            _contextHandle.SampleTailFree(ptr, options.TailFreeZ, 1);
            _contextHandle.SampleTypical(ptr, options.LocalTypicalK, 1);
            _contextHandle.SampleTopP(ptr, options.TopP, 1);
        }

        if (!options.PenalizeNewLine)
        {
            logits[_newLineToken] = newLineLogit;
        }


        const int mirostatM = 100;

        return endSampler switch
        {
            SamplingMethod.Mirostat => SampleTokenMirostat(options, ptr, mirostatM),
            SamplingMethod.MirostatV2 => SampleTokenMirostatV2(options, ptr),
            SamplingMethod.Default => SampleToken(ptr),
            SamplingMethod.Greedy => SampleTokenGreedy(ptr),
            _ => throw new ArgumentOutOfRangeException(nameof(candidatesP))
        };
    }

    private int SampleTokenGreedy(IntPtr ptr)
    {
        return _contextHandle.SampleTokenGreedy(ptr);
    }

    private int SampleToken(IntPtr ptr)
    {
        return _contextHandle.SampleToken(ptr);
    }

    private int SampleTokenMirostatV2(InferenceOptions options, IntPtr ptr)
    {
        var mirostatMu = 2.0f * options.MirostatTau;

        return _contextHandle.SampleTokenMirostatV2(ptr, options.MirostatTau, options.MirostatEta, &mirostatMu);
    }

    private int SampleTokenMirostat(InferenceOptions options, IntPtr ptr, int mirostatM)
    {
        var mirostatMu = 2.0f * options.MirostatTau;


        return _contextHandle.SampleTokenMirostat(ptr, options.MirostatTau, options.MirostatEta, mirostatM,
            &mirostatMu);
    }
}