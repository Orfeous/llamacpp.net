using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;

namespace LlamaCpp.Net.Extensions
{
    internal static class ContextParamsExtensions
    {
        internal static void Apply(ref this LLamaContextParams lparams, LanguageModelOptions options)
        {
            lparams.n_ctx = options.ContextSize;
            lparams.n_gpu_layers = options.GpuLayerCount;
            lparams.seed = options.Seed;
            lparams.f16_kv = options.UseFp16Memory;
            lparams.use_mmap = options.UseMemoryLock;
            lparams.use_mlock = options.UseMemoryLock;
            lparams.embedding = options.EmbeddingMode;
        }
    }
}
