using FluentAssertions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Samplers.Pipelines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using System.Text;

namespace LlamaCpp.Net.Tests;

[TestFixture]
public class LanguageModelTests
{
    private MockRepository _repository;
    private Mock<ISamplingPipeline> _samplingPipelineMock;
    private Mock<ILlamaInstance> _instanceMock;
    private Logger<LanguageModel> _logger;

    [SetUp]
    public void Setup()
    {
        this._repository = new MockRepository(MockBehavior.Strict);

        this._samplingPipelineMock = this._repository.Create<ISamplingPipeline>();
        this._instanceMock = this._repository.Create<ILlamaInstance>();

        this._instanceMock.Setup(x => x.GetVocabSize())
            .Returns(10);
        this._logger = new Logger<LanguageModel>(new NullLoggerFactory());
    }


    public LanguageModel CreateModel()
    {
        return new LanguageModel(_samplingPipelineMock.Object,
            _instanceMock.Object,
            _logger,
            Path.Join(Constants.ModelDirectory, "wizardLM-7B.ggmlv3.q4_0.bin"),
            LanguageModelOptions.Default
        );
    }


    [Test]
    public void Tokenize_ReturnsListOfTokens_WhenTextIsValid()
    {
        // Arrange
        var text = "This is a test.";
        var expectedTokens = new List<int> { 1, 2, 3, 4, 5 };

        _instanceMock.Setup(x => x.Tokenize(text, It.IsAny<int[]>(), It.IsAny<int>(), true))
            .Returns(expectedTokens.Count);

        _instanceMock.Setup(instance => instance.TokenToString(It.IsAny<int>(), It.IsAny<Encoding>()))
            .Returns("{token}");


        var languageModel = CreateModel();

        // Act
        var tokens = languageModel.Tokenize(text);

        // Assert

        expectedTokens.Should().BeEquivalentTo(new int[] { 1, 2, 3, 4, 5 });
    }

    [Test]
    public void Tokenize_ThrowsTokenizationFailedException_WhenTextCannotBeTokenized()
    {
        // Arrange
        var text = "This is a test.";


        _instanceMock.Setup(x => x.Tokenize(text, It.IsAny<int[]>(), It.IsAny<int>(), true))
            .Returns(0);

        _instanceMock.Setup(instance => instance.TokenToString(It.IsAny<int>(), It.IsAny<Encoding>()))
            .Returns("{token}");
        var languageModel = CreateModel();
        // Act & Assert
        Assert.Throws<TokenizationFailedException>(() => languageModel.Tokenize(text));
    }

    [Test]
    public void TokenToString_ReturnsStringRepresentationOfToken_WhenTokenIsValid()
    {
        // Arrange
        var token = 1;
        var expectedString = "This";

        _instanceMock.Setup(x => x.TokenToStr(token))
            .Returns(new IntPtr(1));
        _instanceMock.Setup(instance => instance.TokenToString(It.IsAny<int>(), It.IsAny<Encoding>())).Returns("This");

        var languageModel = CreateModel();
        // Act
        var tokenString = languageModel.TokenToString(token);

        // Assert

        expectedString.Should().BeEquivalentTo(tokenString);
    }

    [Test]
    public void TokenToString_ReturnsCachedStringRepresentationOfToken_WhenTokenHasBeenConvertedBefore()
    {
        // Arrange
        var token = 1;
        var expectedString = "This";

        _instanceMock.Setup(instance => instance.TokenToString(It.IsAny<int>(), It.IsAny<Encoding>())).Returns("This");

        var languageModel = CreateModel();

        languageModel.TokenToString(token);

        // Act
        var tokenString = languageModel.TokenToString(token);

        // Assert

        expectedString.Should().BeEquivalentTo(tokenString);
        _instanceMock.Verify(x => x.TokenToString(token, It.IsAny<Encoding>()), Times.Once);
    }
}