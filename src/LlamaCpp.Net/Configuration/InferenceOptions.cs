﻿namespace LlamaCpp.Net.Configuration;

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
    /// Inference options for having a conversation with the model
    /// </summary>
    public static InferenceOptions Chat => Default with
    {
        MaxNumberOfTokens = 200,
        Temperature = 0.8f,
        RepetitionPenalty = 1.1f,
        TopK = 0,
        TopP = 0.7f
    };

    /// <summary>
    /// Inference options for getting precise results from the model
    /// </summary>
    public static InferenceOptions Precise => Default with
    {
        Temperature = 0.7f,
        RepetitionPenalty = 1.1f,
        TopK = 40,
        TopP = 0.1f
    };

    /// <summary>
    /// 
    /// </summary>

    public float TopP { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int TopK { get; set; }

    /// <summary>
    ///     The number of tokens to generate
    ///     -1 means to generate until the model decides to stop
    ///     Do note that the model may generate less tokens than this number when a stop token is generated
    /// </summary>
    public int MaxNumberOfTokens { get; init; } = 120;


    /// <summary>
    ///     The number of threads to use
    /// </summary>
    public int Threads { get; init; } = 4;

    /// <summary>
    ///     The RepetitionPenalty parameter helps prevent the model from generating repetitive or monotonous text.
    ///     A higher value (e.g., 1.5) will penalize repetitions more strongly,
    ///     while a lower value (e.g., 0.9) will be more lenient. The default value is 1.1.
    /// </summary>
    /// <remarks>
    ///     Used by the repetition penalty sampler,
    ///     Used by the frequency and presence penalty sampler
    /// </remarks>
    public float RepetitionPenalty { get; init; } = 1.1f;

    /// <summary>
    ///     The RepetitionLastN parameter controls the number of previous tokens to consider when applying the repetition
    ///     A larger value will make the model look further back in history when checking for repetitions, while a smaller
    ///     value will make the model more... forgetful.
    ///     A zero value will disable the repetition penalty, while a value of -1 will make the model consider all previous
    ///     tokens when checking for repetitions (within the context window).
    /// </summary>
    public ulong RepetitionLastN { get; set; }

    /// <summary>
    ///     Whether to penalize newlines.
    ///     This will make the model less likely to generate newlines.
    ///     This prevents the model from generating the entire bible at once, for example.
    /// </summary>
    public bool PenalizeNewLine { get; init; } = true;

    /// <summary>
    ///     The number of tokens to look back 
    /// </summary>
    public int TokenLookback { get; init; } = 2;

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

    /// <summary>
    ///     The temperature parameter. Temperature controls the randomness of the model's output.
    ///     It affects the probability distribution of the model's output tokens.
    ///     A higher temperature (e.g., 1.5) makes the output more random and creative,
    ///     while a lower temperature (e.g., 0.5) makes the output more focused, deterministic, and conservative.
    ///     The default value is 0.8, which provides a balance between randomness and determinism.
    ///     At the extreme, a temperature of 0 will always pick the most likely next token, leading to identical outputs in
    ///     each run.
    /// </summary>
    /// <remarks>
    ///     Used by the temperature sampler to control the randomness of the model's output.
    /// </remarks>
    public float Temperature { get; set; } = 0.8f;

    /// <summary>
    /// 
    /// </summary>
    public bool TreatNewLineAsEndOfText { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong MinKeep { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int TailFreeZ { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int LocalTypicalK { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SamplingMethod SamplingMethod { get; set; }
}

/// <summary>
/// 
/// </summary>
public enum SamplingMethod
{
    /// <summary>
    /// Random sampling
    /// </summary>
    Default,
    /// <summary>
    /// Mirostat sampling
    /// </summary>
    Mirostat,

    /// <summary>
    /// Mirosat V2 sampling
    /// </summary>
    MirostatV2

}