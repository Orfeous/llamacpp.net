using System;

namespace LlamaCpp.Net.Native;

/// <summary>
/// todo
/// </summary>
public delegate void LlamaProgressCallback(float progress, IntPtr ctx);