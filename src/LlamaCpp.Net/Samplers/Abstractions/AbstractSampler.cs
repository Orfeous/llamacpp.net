using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using System;

namespace LlamaCpp.Net.Samplers.Abstractions;

/// <summary>
///    Abstract sampler class
/// </summary>
internal abstract class AbstractSampler : ISampler
{
    private readonly SafeLLamaContextHandle _context;

    protected AbstractSampler(SafeLLamaContextHandle context)
    {
        _context = context;
    }


    public void Sample(TokenDataArray candidates)
    {
        WrapNativeCall(candidates);
    }


    private unsafe void WrapNativeCall(
        TokenDataArray candidates
    )
    {
        var handle = candidates.data.Pin();
        var st = new TokenDataArrayNative
        {
            data = new IntPtr(handle.Pointer),
            size = candidates.size,
            sorted = candidates.sorted
        };
        Sample(_context, new IntPtr(&st));
    }

    protected abstract void Sample(SafeLLamaContextHandle context, IntPtr intPtr);
}