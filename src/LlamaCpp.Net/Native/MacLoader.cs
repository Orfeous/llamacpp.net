using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
    internal static class MacLoader
    {
        private const int RTLD_LAZY = 0x00001;

        [DllImport("libSystem.dylib", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libSystem.dylib", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        internal static void LibraryLoad(string libraryPath)
        {
            IntPtr libHandle = dlopen(libraryPath, RTLD_LAZY);
            if (libHandle == IntPtr.Zero)
            {
                throw new InvalidComObjectException($"Failed to load {libraryPath}");
            }
        }
    }
}