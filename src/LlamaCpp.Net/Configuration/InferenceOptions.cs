namespace LlamaCpp.Net.Configuration;

/// <summary>
///     Represents the options used for generating text with an AI model.
/// </summary>
public record InferenceOptions
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
    /// 
    /// </summary>
    public SamplingMethod SamplingMethod { get; set; }

    /// <summary>
    /// When using one of the repetition penalty samplers, this value is used to determine how far to look back.
    /// </summary>
    public int RepetitionLookback { get; set; } = 10;

    public float Temperature { get; set; } = 1.0f;
    public int TopK { get; set; }
    public ulong TopKMinKeep { get; set; }
    public ulong TailFreeMinKeep { get; set; }
    public int TailFreeZ { get; set; }
    public int LocalTypicalK { get; set; }
    public ulong LocalTypicalMinKeep { get; set; }
    public float TopP { get; set; }
    public ulong TopPMinKeep { get; set; }
    public string? PromptPrefix { get; set; }
    public string? PromptSuffix { get; set; }
}