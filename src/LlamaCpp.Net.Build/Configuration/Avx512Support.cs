using System;

namespace LlamaCpp.Net.Build.Configuration
{
    [Flags]
    public enum Avx512Support
    {
        None = 0,
        Avx512 = 1,
        Vbmi = 2,
        Vnni = 4
    }
}
