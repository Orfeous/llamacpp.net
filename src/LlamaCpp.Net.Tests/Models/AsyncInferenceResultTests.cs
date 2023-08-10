using FluentAssertions;
using LlamaCpp.Net.Models;
using NUnit.Framework;

namespace LlamaCpp.Net.Tests.Models;

public class AsyncInferenceResultTests
{
    [Test]
    public async Task ToAsyncEnumerable_ReturnsExpectedValues()
    {
        // Arrange
        var result = new AsyncInferenceResult();
        result.Append("foo");
        result.Append("bar");
        result.Complete();

        // Act
        var values = await result.ToAsyncEnumerable().ToListAsync();


        // Assert
        values.Should().BeEquivalentTo("foo", "bar");
    }
}