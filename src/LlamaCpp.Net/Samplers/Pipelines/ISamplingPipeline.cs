using System;
using LlamaCpp.Net.Native.Models;

namespace LlamaCpp.Net.Samplers.Pipelines;

/// <summary>
///     Represents a set of constraints that can be applied to token candidates based on the last tokens and inference
///     options.
/// </summary>
public interface ISamplingPipeline
{
    /// <summary>
    ///     Applies constraints to the given token candidates based on the last tokens and inference options.
    /// </summary>
    /// <param name="candidatesP">The token candidates to apply constraints to.</param>
    /// <param name="logits">The logits for each token.</param>
    /// <param name="currentOutput"></param>
    /// <param name="inferenceOptions">The inference options to use for applying constraints.</param>
    /// <param name="penalizeNewLine"></param>
    /// <param name="repetitionLookback"></param>
    /// <param name="samplingMethod"></param>
    internal int ApplyConstraints(TokenDataArray candidatesP,
        Span<float> logits,
        int[] currentOutput,
        bool penalizeNewLine = false,
        int repetitionLookback = 0
    );
}