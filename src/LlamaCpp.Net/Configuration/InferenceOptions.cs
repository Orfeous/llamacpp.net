namespace LlamaCpp.Net.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public record InferenceOptions
    {
        /// <summary>
        /// The default inference options
        /// </summary>
        public static InferenceOptions Default => new InferenceOptions();

        /// <summary>
        /// The number of tokens to generate
        /// </summary>
        public int MaxNumberOfTokens { get; init; } = 120;

        /// <summary>
        /// The number of tokens to look back
        /// </summary>
        public int Past { get; init; } = 1;
        /// <summary>
        /// The number of threads to use
        /// </summary>
        public int Threads { get; init; } = 4;
        /// <summary>
        /// How much to penalize repetition
        /// </summary>
        public float RepetitionPenalty { get; init; } = 1.1f;

        /// <summary>
        /// Whether to penalize newlines
        /// </summary>
        public bool PenalizeNewLine { get; init; } = true;

        /// <summary>
        ///  The number of tokens to repeat
        /// </summary>
        public int RepeatLastTokensCount { get; init; } = 64;
    }
}
