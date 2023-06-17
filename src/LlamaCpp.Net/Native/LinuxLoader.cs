using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
    /// <summary>
    /// Loader class to load the linux specific llama.cpp library
    /// </summary>
    internal static class LinuxLoader
    {
        private const int RTLD_LAZY = 0x00001;

        [DllImport("libdl.so", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so.2", EntryPoint = "dlopen", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlopen2(string filename, int flags);

        internal static void LibraryLoad(string libraryPath)
        {
            IntPtr libHandle;
            try
            {
                libHandle = dlopen(libraryPath, RTLD_LAZY);
            }
            catch (DllNotFoundException)
            {
                libHandle = dlopen2(libraryPath, RTLD_LAZY);
            }

            if (libHandle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Failed to load {libraryPath}");
            }
        }
    }
}