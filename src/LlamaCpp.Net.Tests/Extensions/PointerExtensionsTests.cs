using FluentAssertions;
using LlamaCpp.Net.Extensions;
using NUnit.Framework;
using System.Runtime.InteropServices;
using System.Text;

namespace LlamaCpp.Net.Tests.Extensions;

public class PointerExtensionsTests
{
    [Test]
    public void PtrToString_ReturnsString_WhenEncodingIsUTF8()
    {
        // Arrange
        var expected = "Hello, world!";
        var bytes = Encoding.UTF8.GetBytes(expected);
        var ptr = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteByte(ptr + bytes.Length, 0);

        // Act
        var actual = ptr.PtrToString(Encoding.UTF8);

        // Assert
        actual.Should().Be(expected);

        // Cleanup
        Marshal.FreeHGlobal(ptr);
    }

    [Test]
    public void PtrToString_ReturnsString_WhenEncodingIsUnicode()
    {
        // Arrange
        var expected = "Hello, world!";
        var bytes = Encoding.Unicode.GetBytes(expected);
        var ptr = Marshal.AllocHGlobal(bytes.Length + 2);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteInt16(ptr + bytes.Length, 0);

        // Act
        var actual = ptr.PtrToString(Encoding.Unicode);

        // Assert
        actual.Should().NotBe(expected);

        // Sadly, this fails to work. But i'm making a test for it, so we know if it ever starts working.


        // Cleanup
        Marshal.FreeHGlobal(ptr);
    }

    [Test]
    public void PtrToString_ReturnsString_WhenEncodingIsDefault()
    {
        // Arrange
        var expected = "Hello, world!";
        var bytes = Encoding.Default.GetBytes(expected);
        var ptr = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteByte(ptr + bytes.Length, 0);

        // Act
        var actual = ptr.PtrToString(Encoding.Default);

        // Assert
        actual.Should().Be(expected);

        // Cleanup
        Marshal.FreeHGlobal(ptr);
    }
}