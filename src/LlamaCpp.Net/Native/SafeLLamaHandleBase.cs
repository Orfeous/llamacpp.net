using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
    /// <summary>
    /// todo
    /// </summary>
    internal abstract class SafeLLamaHandleBase : SafeHandle
    {
        /// <summary>
        /// todo
        /// </summary>
        protected internal SafeLLamaHandleBase() : base(IntPtr.Zero, ownsHandle: true) { }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="handle"></param>
        private protected SafeLLamaHandleBase(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ownsHandle"></param>
        private protected SafeLLamaHandleBase(IntPtr handle, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// todo
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"0x{handle.ToString("x16")}";
    }
}
