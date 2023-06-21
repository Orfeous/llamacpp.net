﻿using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

// 
/// <summary>
///     Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
///     TFS  is a text generation technique that aims to reduce the impact of less likely tokens, which may be less
///     relevant, less coherent, or nonsensical, on the output.
///     The method adjusts the logits (token probabilities) by raising them to the power of the parameter z.
///     A higher value of z (e.g., 2.0) will further suppress less likely tokens from the tail of the distribution, while a
///     value of 1.0 disables the effect of TFS.
///     By setting the parameter z, you can control how much the probabilities of less likely tokens are reduced
/// </summary>
internal sealed class TailFreeSampler : AbstractSampler
{
    private readonly int _z;
    private readonly ulong _minKeep;

    private TailFreeSampler(SafeLLamaContextHandle context, InferenceOptions options) : base(context)
    {
        _z = options.TailFreeZ;
        _minKeep = options.MinKeep;
    }


    public override void Sample(IntPtr intPtr)
    {
        _context.llama_sample_tail_free(intPtr, _z, _minKeep);
    }
}