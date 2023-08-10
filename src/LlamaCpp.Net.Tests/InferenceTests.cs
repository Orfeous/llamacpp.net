using System.Diagnostics;
using FluentAssertions;
using LlamaCpp.Net.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace LlamaCpp.Net.Tests;

public class InferenceTests
{
    [Test]
    public void Infer_ForSimpleSum_ShouldReturnAnswer()
    {
        var languageModel = new LanguageModel(
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin")
        );

        const string prompt = "2 + 2 = ";
        var options = new InferenceOptions()
        {
            SamplingMethod = SamplingMethod.Greedy,
            MaxNumberOfTokens = 2
        };


        var result = languageModel.Infer(prompt, options);

        result.Trim().Should().BeEquivalentTo("4");
    }

    [Test]
    public void Infer_ForSimpleLogicalQuestion_ShouldReturnAnswer()
    {
        var languageModel = new LanguageModel(
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin")
        );
        const string prompt = "Is the sky blue?";
        var options = new InferenceOptions()
        {
            SamplingMethod = SamplingMethod.Greedy,
            MaxNumberOfTokens = 2
        };

        languageModel.Infer(prompt, options).Trim().Should().BeEquivalentTo("Yes");
    }

    [Test]
    public void Infer_ForSimpleLogicalQuestion_ShouldReturnAnswer_WhenGivenContradictoryData()
    {
        var languageModel = new LanguageModel(
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin")
        );
        const string prompt = "The sky is red. Is the sky blue?";
        var options = new InferenceOptions()
        {
            SamplingMethod = SamplingMethod.Greedy,
            MaxNumberOfTokens = 2
        };

        languageModel.Infer(prompt, options).Trim().Should().BeEquivalentTo("No");
    }

    [Test]
    public void Infer_ShortStory()
    {
        var languageModel = new LanguageModel(
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin")
        );

        const string prompt = "When Sean was young, ";
        var options = new InferenceOptions()
        {
            SamplingMethod = SamplingMethod.Greedy,
            MaxNumberOfTokens = 100
        };

        var result = languageModel.Infer(prompt, options);

        result.Should().NotBeNullOrEmpty();

        result.Should()
            .Contain(
                "he was fascinated by the idea of time travel. He would spend hours reading books and watching movies about it, dreaming of the day when he could finally experience it for himself.");
    }

    public ILogger<LanguageModel> CreateLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        return loggerFactory.CreateLogger<LanguageModel>();
    }

    [Test]
    public void Infer_ShouldStopAtAntiPrompt()
    {
        var languageModelOptions = new LanguageModelOptions
        {
            UseMemoryLock = true,
            ContextSize = 4196,
        };
        var inferenceOptions = new InferenceOptions
        {
            SamplingMethod = SamplingMethod.Default,
            MaxNumberOfTokens = 40,

            PromptPrefix = "USER: ",
            PromptSuffix = "\nASSISTANT: ",
            Antiprompts = new List<string>()
            {
                "USER:",
                "ASSISTANT:"
            }
        };

        var languageModel = new LanguageModel(
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin"), languageModelOptions, CreateLogger()
        );


        var result = languageModel.Infer("Good evening!", inferenceOptions);

        Trace.WriteLine(result);

        result.Should().NotContain("USER:");
        result.Should().NotContain("ASSISTANT:");
    }

    [Test]
    public void Infer_ShouldNotStartWithWhitespace()
    {
        var languageModelOptions = new LanguageModelOptions
        {
            UseMemoryLock = true,
            ContextSize = 4196,
        };
        var inferenceOptions = new InferenceOptions
        {
            SamplingMethod = SamplingMethod.Default,
            MaxNumberOfTokens = 20,

            PromptPrefix = "USER: ",
            PromptSuffix = "\nASSISTANT: ",
            Antiprompts = new List<string>()
            {
                "USER:",
                "ASSISTANT:"
            }
        };

        var languageModel = new LanguageModel(
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin"), languageModelOptions, CreateLogger()
        );

        languageModel.Infer("Good evening!", inferenceOptions).Should().NotStartWith(" ");
    }

}