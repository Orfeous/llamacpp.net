using System;

namespace LlamaCpp.Net.Native
{
    /// <summary>
    /// todo
    /// </summary>
    internal sealed unsafe class SafeLLamaContextHandle : SafeLLamaHandleBase
    {
        /// <summary>
        /// todo
        /// </summary>
        internal SafeLLamaContextHandle()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownsHandle"></param>
        internal SafeLLamaContextHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
        {
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="handle"></param>
        internal SafeLLamaContextHandle(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            LlamaNative.llama_free(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }


    }
}