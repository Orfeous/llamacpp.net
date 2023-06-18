namespace LlamaCpp.Net.Configuration
{
    /// <summary>
    /// A class to hold the options for the language model
    /// </summary>
    public record LanguageModelOptions
    {
        /// <summary>
        /// Default options for the language model
        /// </summary>
        public static LanguageModelOptions Default => new LanguageModelOptions
        {
            ContextSize = 1024,
            GpuLayerCount = 24,
            Seed = -1,
            UseFp16Memory = false,
            UseMemoryLock = false,
            EmbeddingMode = false
        };

        /// <summary>
        /// The context size for the language model
        /// </summary>
        public int ContextSize { get; init; }

        /// <summary>
        /// The number of gpu layers for the language model
        /// </summary>
        public int GpuLayerCount { get; init; }

        /// <summary>
        /// The seed for the language model
        /// </summary>
        public int Seed { get; init; }

        /// <summary>
        /// Whether to use fp16 memory
        /// </summary>
        public bool UseFp16Memory { get; init; }

        /// <summary>
        /// Whether to use memory lock
        /// </summary>
        public bool UseMemoryLock { get; init; }

        /// <summary>
        /// Whether to use embedding mode
        /// </summary>
        public bool EmbeddingMode { get; init; }

        /// <summary>
        /// The path to the lora adapter
        /// </summary>
        public string? LoraAdapterPath { get; init; } = string.Empty;

        /// <summary>
        /// The number of lora threads
        /// </summary>
        public int LoraThreads { get; init; } = 20;
    }
}
