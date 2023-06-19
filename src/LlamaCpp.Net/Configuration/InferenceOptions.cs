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
    /// </summary>
    public int MaxNumberOfTokens { get; init; } = 120;

    /// <summary>
    ///     The number of tokens to look back
    /// </summary>
    public int Past { get; init; } = 1;

    /// <summary>
    ///     The number of threads to use
    /// </summary>
    public int Threads { get; init; } = 4;

    /// <summary>
    ///     How much to penalize repetition
    /// </summary>
    public float RepetitionPenalty { get; init; } = 1.1f;

    /// <summary>
    ///     Whether to penalize newlines
    /// </summary>
    public bool PenalizeNewLine { get; init; } = true;

    /// <summary>
    ///     The number of tokens to repeat
    /// </summary>
    public int RepeatLastTokensCount { get; init; } = 64;

    /// <summary>
    ///     The presence of the alpha parameter. Alpha controls the degree of randomness in the model's output.
    ///     A reasonable value for this parameter is between 0.1 and 1, with 1 being the most random.
    /// </summary>
    public float AlphaPresence { get; set; } = 0.1f;

    /// <summary>
    ///     The frequency of the alpha parameter. Alpha controls the degree of randomness in the model's output.
    ///     A reasonable value for this parameter is between 0.1 and 1, with 1 being the most random.
    /// </summary>
    public float AlphaFrequency { get; set; } = 0.1f;
}