using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LlamaCpp.Net.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace LlamaCpp.Net.Test
{
    [TestFixture]
    public class LanguageModelTests
    {
        public ILanguageModel CreateInstance()
        {
            var modelFileName = "wizardLM-7B.ggmlv3.q4_0.bin";
            var modelPath = Path.Join(Constants.ModelDirectory, modelFileName);
            modelPath = Path.GetFullPath(modelPath);
            return new LanguageModel(modelPath, new Logger<LanguageModel>(new NullLoggerFactory()));
        }

        [Test]
        public void CanLoadModel()
        {
            var instance = CreateInstance();

            Assert.NotNull(instance);
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
