using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
    /// <summary>
    /// Loader class to load the windows specific llama.cpp library
    /// </summary>
    internal static class WindowsLoader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        internal static void LibraryLoad(string libraryPath)
        {
            IntPtr libHandle = LoadLibrary(libraryPath);
            if (libHandle == IntPtr.Zero)
            {
                throw new InvalidComObjectException($"Failed to load {libraryPath}");
            }
        }
    }
}