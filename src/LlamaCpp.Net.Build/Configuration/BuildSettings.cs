using Cake.Core;
using Cake.Core.IO;
using LlamaCpp.Net.Build.Extensions;

namespace LlamaCpp.Net.Build.Configuration
{
    public abstract record BuildSettings
    {
        public string? FriendlyName { get; init; }
        public string Platform { get; init; } = "x64";

        public DirectoryPath BuildPath => GetDirectoryPath();
        public BlasType BlasType { get; init; }

        public bool Standalone { get; init; }
        public bool EnableKQuants { get; init; }

        public Avx512Support Avx512Support { get; init; } = Avx512Support.None;

        public bool EnableLinkTimeOptimization { get; set; }

        public abstract string CompilerType { get; }
        public string BuildConfiguration { get; set; } = "Release";

        public DirectoryPath GetDirectoryPath()

        {
            if (FriendlyName != null)
            {
                return new DirectoryPath("build-" + CompilerType + "-" + FriendlyName);
            }

            var path = $"build-msvc-{Platform}-{BlasType.AsString()}";

            if (EnableKQuants)
            {
                path += "-kquants";
            }
            else
            {
                path += "-no-kquants";
            }

            if (Avx512Support != Avx512Support.None)
            {
                path += "-avx512";

                if (Avx512Support.HasFlag(Avx512Support.Vbmi))
                {
                    path += "-vbmi";
                }

                if (Avx512Support.HasFlag(Avx512Support.Vnni))
                {
                    path += "-vnni";
                }
            }
            else
            {
                path += "-no-avx512";
            }

            if (EnableLinkTimeOptimization)
            {
                path += "-lto";
            }
            else
            {
                path += "-no-lto";
            }

            var directoryPath = new DirectoryPath(path);

            return directoryPath;
        }

        public void Apply(ProcessArgumentBuilder processParameterBuilder)
        {
            processParameterBuilder.Append("-A ");
            processParameterBuilder.Append(Platform);

            processParameterBuilder.AddBlasType(BlasType);

            processParameterBuilder.AppendCmakeOption("LLAMA_STANDALONE", Standalone);
            if (EnableKQuants)
            {
                processParameterBuilder.AppendCmakeOption("LLAMA_K_QUANTS", true);
            }

            if (EnableLinkTimeOptimization)
            {
                processParameterBuilder.AppendCmakeOption("LLAMA_LTO", true);
            }

            if (Avx512Support != Avx512Support.None)
            {
                processParameterBuilder.AppendCmakeOption("LLAMA_AVX512", true);

                if (Avx512Support.HasFlag(Avx512Support.Vbmi))
                {
                    processParameterBuilder.AppendCmakeOption("LLAMA_AVX512_VBMI", true);
                }

                if (Avx512Support.HasFlag(Avx512Support.Vnni))
                {
                    processParameterBuilder.AppendCmakeOption("LLAMA_AVX512_VNNI", true);
                }
            }
        }
    }
}
