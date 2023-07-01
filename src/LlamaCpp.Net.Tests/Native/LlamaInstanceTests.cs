using FluentAssertions;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Native.API;
using NUnit.Framework;


namespace LlamaCpp.Net.Tests.Native;

[TestFixture]
public class LlamaInstanceTests
{
    internal static ILlamaInstance CreateInstance()
    {
        var modelFileName = "wizardLM-7B.ggmlv3.q4_0.bin";
        var modelPath = Path.Join(Constants.ModelDirectory, modelFileName);
        modelPath = Path.GetFullPath(modelPath);

        var contextParams = LlamaNative.llama_context_default_params();
        return new LlamaInstance(modelPath, contextParams);
    }

    [Test]
    public void CanLoadModel()
    {
        using var instance = CreateInstance();

        instance.GetContextSize().Should().BeGreaterThan(0);


    }


}