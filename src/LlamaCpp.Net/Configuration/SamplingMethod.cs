﻿namespace LlamaCpp.Net.Configuration;

/// <summary>
/// 
/// </summary>
public enum SamplingMethod
{
    /// <summary>
    /// Random sampling
    /// </summary>
    Default,
    /// <summary>
    /// Mirostat sampling
    /// </summary>
    Mirostat,

    /// <summary>
    /// Mirosat V2 sampling
    /// </summary>
    MirostatV2

}