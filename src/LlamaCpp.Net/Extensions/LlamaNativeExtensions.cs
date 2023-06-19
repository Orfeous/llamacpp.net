using System;
using LlamaCpp.Net.Native;

namespace LlamaCpp.Net.Extensions
{
    /// <summary>
    /// Extensions for the native library
    /// </summary>
    public static class LlamaNativeExtensions
    {
        internal static unsafe int llama_sample_token(this SafeLLamaContextHandle ctx, TokenDataArray candidates)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            return ctx.llama_sample_token(new IntPtr(&st));
        }

        internal static unsafe void llama_sample_temperature(this SafeLLamaContextHandle ctx, TokenDataArray candidates,
            float temp)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            ctx.llama_sample_temperature(new IntPtr(&st), temp);
        }
    }
}
