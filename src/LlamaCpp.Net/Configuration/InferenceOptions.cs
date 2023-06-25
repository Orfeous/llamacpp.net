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
}