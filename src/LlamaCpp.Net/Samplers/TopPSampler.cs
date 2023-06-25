using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration"
///     https://arxiv.org/abs/1904.09751
///     Top-p sampling, also known as nucleus sampling, is a text generation method that selects the next token
///     from a subset of tokens that together have a cumulative probability of at least p.
///     This method provides a balance between diversity and quality by considering both the probabilities of tokens and
///     the number of tokens to sample from.
///     A higher value for top-p (e.g., 0.95) will lead to more diverse text, while a lower value (e.g., 0.5) will generate
///     more focused and conservative text.
///     The default value for llama.cpp is 0.9.
/// </summary>
internal sealed class TopPSampler : AbstractSampler
{
    private readonly ulong _minKeep;
    private readonly float _p;

    public TopPSampler(SafeLLamaContextHandle context, InferenceOptions options, ulong minKeep) : base(context)
    {
        _minKeep = minKeep;
        _p = options.TopP;
    }


    public override void Sample(IntPtr intPtr)
    {
        _context.llama_sample_top_p(intPtr, _p, _minKeep);
    }
}