using System;
using System.Collections.Generic;
using System.Text;

namespace LlamaCpp.Net.Extensions
{
    /// <summary>
    /// Convenience extensions for pointers
    /// </summary>
    public static class PointerExtensions
    {
        /// <summary>
        ///    Converts a stringPointer to a string
        /// </summary>
        /// <param name="stringPointer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static unsafe string PtrToString(this IntPtr stringPointer, Encoding encoding)
        {
#if NET6_0_OR_GREATER
            if(encoding == Encoding.UTF8)
            {
                return Marshal.PtrToStringUTF8(stringPointer);
            }
            else if(encoding == Encoding.Unicode)
            {
                return Marshal.PtrToStringUni(stringPointer);
            }
            else
            {
                return Marshal.PtrToStringAuto(stringPointer);
            }
#else
            var tp = (byte*)stringPointer.ToPointer();
            List<byte> bytes = new();
            while (true)
            {
                var c = *tp++;
                if (c == '\0')
                {
                    break;
                }

                bytes.Add(c);
            }

            return encoding.GetString(bytes.ToArray());
#endif
        }
    }
}
