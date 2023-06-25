using FluentAssertions;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Diagnostics;
using System.Text;

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace LlamaCpp.Net.Test
{
    [SetUpFixture]
    public class SetupTrace
    {
        [OneTimeSetUp]
        public void StartTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        [OneTimeTearDown]
        public void EndTest()
        {
            Trace.Flush();
        }
    }

    [TestFixture]
    public class LanguageModelTests
    {
        public static ILanguageModel CreateInstance()
        {
            var modelFileName = "wizardLM-7B.ggmlv3.q4_0.bin";
            var modelPath = Path.Join(Constants.ModelDirectory, modelFileName);
            modelPath = Path.GetFullPath(modelPath);
            return new LanguageModel(modelPath, new Logger<LanguageModel>(new NullLoggerFactory()),
                LanguageModelOptions.Default);
        }


        [Test]
        public void Tokenize_Should_ReturnExpectedTokens()
        {
            var instance = CreateInstance();

            var input = "This is a test";

            var tokens = instance.Tokenize(input);

            Assert.NotNull(tokens);
            Assert.IsNotEmpty(tokens);

            var expectedTokens = new int[] { 1, 4013, 338, 263, 1243, };
            tokens.Should().BeEquivalentTo(expectedTokens);
        }

        [Test]
        public void TokenToString_Should_ReturnExpectedString()
        {
            var instance = CreateInstance();
            var input = "This is a test";
            var tokens = instance.Tokenize(input);

            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                sb.Append(instance.TokenToString(token));
            }


            sb.ToString().Should().Be(input);
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores