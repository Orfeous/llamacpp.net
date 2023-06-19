using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native.Loaders
{
    /// <summary>
    /// Helper class to load the OS specific llama.cpp library
    /// </summary>
    internal static class LibraryLoader
    {
        /// <summary>
        /// Name of the native library
        /// 
        /// By default when you build llama.cpp this is called llama.{dll/so/dylib} respectively
        /// </summary>
        internal const string NativeLibraryName = "llama";

        /// <summary>
        /// Helper method to load the OS specific llama.cpp library
        /// </summary>
        internal static void LibraryLoad()
        {
            //* Use windows as default
            var os = "win";
            var ext = "dll";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "win";
                ext = "dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "linux";
                ext = "so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "osx";
                ext = "dylib";
            }


            var libraryPath = Path.Combine(AppContext.BaseDirectory, $"{NativeLibraryName}.{ext}");
            switch (os)
            {
                case "win":
                    WindowsLoader.LibraryLoad(libraryPath);
                    break;
                case "linux":
                    LinuxLoader.LibraryLoad(libraryPath);
                    break;
                case "osx":
                    MacLoader.LibraryLoad(libraryPath);
                    break;
            }
        }
    }
}
