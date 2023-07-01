using LlamaCpp.Net.Native.Loaders;
using NUnit.Framework;

namespace LlamaCpp.Net.Tests.Native.Loaders;

[TestFixture]
public class WindowsLoaderTests
{
    [Test]
    public void LibraryLoad_LoadsLibrary_Success()
    {
        var libraryPath = Path.Combine(AppContext.BaseDirectory, "llama.dll");

        // Arrange

        // Act
        Assert.DoesNotThrow(() => WindowsLoader.LibraryLoad(libraryPath));

        // Assert
        // No exception is thrown
    }

    [Test]
    public void LibraryLoad_LoadsNonExistentLibrary_ThrowsDllNotFoundException()
    {
        // Arrange
        var libraryPath = "path/to/nonexistent/library.dll";

        // Act & Assert
        Assert.Throws<DllNotFoundException>(() => WindowsLoader.LibraryLoad(libraryPath));
    }
}