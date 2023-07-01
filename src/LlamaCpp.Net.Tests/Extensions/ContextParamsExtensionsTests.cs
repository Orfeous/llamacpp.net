using FluentAssertions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Extensions;
using LlamaCpp.Net.Native;
using NUnit.Framework;

namespace LlamaCpp.Net.Tests.Extensions;

[TestFixture]
public class ContextParamsExtensionsTests
{
    [Test]
    public void Apply_Should_Apply_Options_To_Context_Params()
    {
        // Arrange
        var options = new LanguageModelOptions
        {
            ContextSize = 512,
            GpuLayerCount = 8,
            Seed = 1234,
            UseFp16Memory = true,
            UseMemoryLock = true
        };

        var contextParams = new LLamaContextParams();

        contextParams.Apply(options);

        // Assert
        contextParams.n_ctx.Should().Be(options.ContextSize);
        contextParams.n_gpu_layers.Should().Be(options.GpuLayerCount);
        contextParams.seed.Should().Be(options.Seed);
        contextParams.f16_kv.Should().Be(options.UseFp16Memory);
        contextParams.use_mmap.Should().Be(options.UseMemoryLock);
        contextParams.use_mlock.Should().Be(options.UseMemoryLock);
    }
}