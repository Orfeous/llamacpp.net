using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Samplers.Abstractions;
using System;

namespace LlamaCpp.Net.Samplers;

/// <summary>
///     Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration"
///     https://arxiv.org/abs/1904.09751
///     Top-k sampling is a text generation method that selects the next token only from the top k most likely tokens
///     predicted by the model.
///     It helps reduce the risk of generating low-probability or nonsensical tokens, but it may also limit the diversity
///     of the output.
///     A higher value for top-k (e.g., 100) will consider more tokens and lead to more diverse text, while a lower value
///     (e.g., 10) will focus on the most probable tokens and generate more conservative text.
///     A reasonable value, and the default for Llama.cpp is 40.
/// </summary>
internal sealed class TopKSampler : AbstractSampler
{
    private readonly int _k;
    private readonly ulong _minKeep;

    public TopKSampler(SafeLLamaContextHandle context, InferenceOptions options, ulong i) : base(context)
    {
        _k = options.TopK;
        _minKeep = i;
    }


    public override void Sample(IntPtr intPtr)
    {
        _context.llama_sample_top_k(intPtr, _k, _minKeep);
    }
}