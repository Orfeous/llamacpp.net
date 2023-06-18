using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
    /// <summary>
    ///    LLama token data array
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct TokenDataArray
    {
        public Memory<TokenData> data;
        public ulong size;
        [MarshalAs(UnmanagedType.I1)] public bool sorted;

        public TokenDataArray(TokenData[] data, ulong size, bool sorted)
        {
            this.data = data;
            this.size = size;
            this.sorted = sorted;
        }
    }
}
