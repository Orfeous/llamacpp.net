using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native.Models
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TokenDataArrayNative
    {
        public IntPtr data;
        public ulong size;
        public bool sorted;
    }
}
