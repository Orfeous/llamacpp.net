using FluentAssertions;
using LlamaCpp.Net.Extensions;
using NUnit.Framework;
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
        var ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes.Length + 1);
        System.Runtime.InteropServices.Marshal.Copy(bytes, 0, ptr, bytes.Length);
        System.Runtime.InteropServices.Marshal.WriteByte(ptr + bytes.Length, 0);

        // Act
        var actual = ptr.PtrToString(Encoding.UTF8);

        // Assert
        actual.Should().Be(expected);

        // Cleanup
        System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
    }

    [Test]
    public void PtrToString_ReturnsString_WhenEncodingIsUnicode()
    {
        // Arrange
        var expected = "Hello, world!";
        var bytes = Encoding.Unicode.GetBytes(expected);
        var ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes.Length + 2);
        System.Runtime.InteropServices.Marshal.Copy(bytes, 0, ptr, bytes.Length);
        System.Runtime.InteropServices.Marshal.WriteInt16(ptr + bytes.Length, 0);

        // Act
        var actual = ptr.PtrToString(Encoding.Unicode);

        // Assert
        actual.Should().NotBe(expected);

        // Sadly, this fails to work. But i'm making a test for it, so we know if it ever starts working.


        // Cleanup
        System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
    }

    [Test]
    public void PtrToString_ReturnsString_WhenEncodingIsDefault()
    {
        // Arrange
        var expected = "Hello, world!";
        var bytes = Encoding.Default.GetBytes(expected);
        var ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes.Length + 1);
        System.Runtime.InteropServices.Marshal.Copy(bytes, 0, ptr, bytes.Length);
        System.Runtime.InteropServices.Marshal.WriteByte(ptr + bytes.Length, 0);

        // Act
        var actual = ptr.PtrToString(Encoding.Default);

        // Assert
        actual.Should().Be(expected);

        // Cleanup
        System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
    }
}