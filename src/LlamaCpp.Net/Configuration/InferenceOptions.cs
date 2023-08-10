using System.Collections.Generic;

namespace LlamaCpp.Net.Configuration;

/// <summary>
///     Represents the options used for generating text with an AI model.
/// </summary>
public class InferenceOptions
{
    /// <summary>
    ///     The default inference options
    /// </summary>
    public static InferenceOptions Default => new InferenceOptions();


    /// <summary>
    ///     The number of tokens to generate
    ///     -1 means to generate until the model decides to stop
    ///     Do note that the model may generate less tokens than this number when a stop token is generated
    /// </summary>
    public int MaxNumberOfTokens { get; init; } = 120;


    /// <summary>
    ///     Whether to penalize newlines.
    ///     This will make the model less likely to generate newlines.
    ///     This prevents the model from generating the entire bible at once, for example.
    /// </summary>
    public bool PenalizeNewLine { get; init; } = true;

    /// <summary>
    /// Whether to compute the perplexity of the prompt.
    /// </summary>
    public bool ComputePerplexity { get; init; } = false;

    /// <summary>
    /// </summary>
    public SamplingMethod SamplingMethod { get; set; } = SamplingMethod.Default;

    /// <summary>
    ///     When using one of the repetition penalty samplers, this value is used to determine how far to look back.
    /// </summary>
    public int RepetitionLookback { get; set; } = 64;

    /// <summary>
    /// 1.0 = disabled
    /// </summary>
    public float Temperature { get; set; } = 1.0f;

    /// <summary>
    /// 0 to use vocab size
    /// </summary>
    public int TopK { get; set; } = 40;

    /// <summary>
    /// 1.0 = disabled
    /// </summary>
    public int TailFreeZ { get; set; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public int LocalTypicalK { get; set; } = 1;

    public float TopP { get; set; } = 0.95f;
    public string? PromptPrefix { get; set; } = string.Empty;
    public string? PromptSuffix { get; set; } = string.Empty;

    /// <summary>
    /// Last N tokens to penalize
    /// </summary>
    public int RepeatLastN { get; set; } = 64;

    /// <summary>
    /// 1.0 = disabled
    /// </summary>
    public float RepeatPenalty { get; set; } = 1.10f;

    /// <summary>
    /// 0.0 = disabled
    /// </summary>
    public float PresencePenalty { get; set; } = 0.00f;

    /// <summary>
    ///  0.0 = disabled
    /// </summary>
    public float FrequencyPenalty { get; set; } = 0.00f;

    /// <summary>
    /// Learning rate
    /// </summary>
    public float MirostatTau { get; set; } = 0.10f;

    /// <summary>
    /// Target entropy
    /// </summary>
    public float MirostatEta { get; set; } = 5.0f;

    /// <summary>
    /// Batch size for prompt processing (must be >=32 to use BLAS)
    /// </summary>
    public int BatchSize { get; set; } = 32;

    /// <summary>
    /// Number of tokens to keep from initial prompt
    /// </summary>
    public int KeepSize { get; set; } = 0;

    public List<string> Antiprompts { get; set; } = new List<string>();
}