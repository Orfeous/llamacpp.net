using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using System;

namespace LlamaCpp.Net.Samplers.Abstractions;

/// <summary>
///     Abstract sampler class that implements the <see cref="ISampler" /> interface.
/// </summary>
internal abstract class AbstractSampler : ISampler

{
    private readonly SafeLLamaContextHandle _context;

    protected AbstractSampler(SafeLLamaContextHandle context)
    {
        _context = context;
    }


    /// <summary>
    ///     Samples the given token data array.
    /// </summary>
    /// <param name="candidates">The token data array to be sampled.</param>
    public void Sample(TokenDataArray candidates)
    {
        WrapNativeCall(candidates);
    }


    /// <summary>
    ///     Wraps the native call to the <see cref="AbstractSampler.Sample(SafeLLamaContextHandle, IntPtr)" /> method with the given <paramref name="candidates" />.
    /// </summary>
    /// <param name="candidates">The token data array to be passed to the native method.</param>
    private unsafe void WrapNativeCall(TokenDataArray candidates)
    {
        // Pin the data array to prevent the garbage collector from moving it around
        var handle = candidates.data.Pin();

        // Create a native representation of the token data array
        var st = new TokenDataArrayNative
        {
            data = new IntPtr(handle.Pointer),
            size = candidates.size,
            sorted = candidates.sorted
        };

        // Call the abstract Sample method with the native representation of the token data array
        Sample(_context, new IntPtr(&st));
    }

    /// <summary>
    ///     Abstract method that must be implemented by derived classes to sample the given token data array.
    /// </summary>
    /// <param name="context">The context handle.</param>
    /// <param name="intPtr">The pointer to the token data array.</param>
    protected abstract void Sample(SafeLLamaContextHandle context, IntPtr intPtr);
}