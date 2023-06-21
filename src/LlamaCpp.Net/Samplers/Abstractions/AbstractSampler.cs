using LlamaCpp.Net.Native;
using System;

namespace LlamaCpp.Net.Samplers.Abstractions;

/// <summary>
///     Abstract sampler class that implements the <see cref="ISampler" /> interface.
/// </summary>
internal abstract class AbstractSampler : ISampler

{
    protected readonly SafeLLamaContextHandle _context;

    protected AbstractSampler(SafeLLamaContextHandle context)
    {
        _context = context;
    }


    public abstract void Sample(IntPtr intPtr);
}