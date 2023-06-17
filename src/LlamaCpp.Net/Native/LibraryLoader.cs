using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
    /// <summary>
    /// 
    /// </summary>
    internal static class LibraryLoader
    {
        /// <summary>
        /// 
        /// </summary>
        public static void LibraryLoad()
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

            var libraryPath = Path.Combine(AppContext.BaseDirectory, $"llama.{ext}");
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
