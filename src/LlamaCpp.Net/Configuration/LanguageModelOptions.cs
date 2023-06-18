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
        public int ContextSize { get; set; }

        /// <summary>
        /// The number of gpu layers for the language model
        /// </summary>
        public int GpuLayerCount { get; set; }

        /// <summary>
        /// The seed for the language model
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Whether to use fp16 memory
        /// </summary>
        public bool UseFp16Memory { get; set; }

        /// <summary>
        /// Whether to use memory lock
        /// </summary>
        public bool UseMemoryLock { get; set; }

        /// <summary>
        /// Whether to use embedding mode
        /// </summary>
        public bool EmbeddingMode { get; set; }

        /// <summary>
        /// The path to the lora adapter
        /// </summary>
        public string? LoraAdapterPath { get; set; } = string.Empty;

        /// <summary>
        /// The number of lora threads
        /// </summary>
        public int LoraThreads { get; set; } = 20;
    }
}
