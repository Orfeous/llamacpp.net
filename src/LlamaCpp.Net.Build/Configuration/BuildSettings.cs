using System;
using Cake.Core;
using Cake.Core.IO;
using LlamaCpp.Net.Build.Extensions;

namespace LlamaCpp.Net.Build.Configuration
{
    public abstract record BuildSettings
    {
        public string FriendlyName { get; init; }
        public string Platform { get; init; } = "x64";

        public DirectoryPath BuildPath => GetDirectoryPath();
        public BlasType BlasType { get; init; }

        public bool Standalone { get; init; } = false;
        public bool EnableKQuants { get; init; } = false;

        public Avx512Support Avx512Support { get; init; } = Avx512Support.None;

        public bool EnableLinkTimeOptimization { get; set; }

        public abstract string CompilerType { get; }
        public string BuildConfiguration { get; set; }

        public string PackageName()
        {
            var packageName = "LlamaCpp.Net.Runtime";


            packageName += BlasType switch
            {
                BlasType.None => ".Cpu",
                BlasType.CuBlas => ".Cuda",
                BlasType.OpenBlas => ".OpenBlas",
                BlasType.CLBlast => ".ClBlast",
                BlasType.Blis => ".Blis",
                _ => throw new ArgumentOutOfRangeException()
            };

            if (BlasType == BlasType.OpenBlas)
            {
                packageName += "." + OpenBlasVendor.ToString();

            }
            if (EnableKQuants)
            {
                packageName += ".KQuants";
            }

            if (Avx512Support != Avx512Support.None)
            {
                packageName += ".Avx512";
            }

            return packageName;
        }

        public DirectoryPath GetDirectoryPath()

        {
            if (!string.IsNullOrWhiteSpace(FriendlyName))
            {
                return new DirectoryPath("build-" + CompilerType + "-" + FriendlyName);
            }

            var path = PackageName();

            var directoryPath = new DirectoryPath(path);

            return directoryPath.Combine(Platform);
        }

        public string GetName()
        {
            var path = $"build-msvc-{BlasType.AsString()}";


            if (Avx512Support != Avx512Support.None)
            {
                path += "-avx512";
            }
            else
            {
                path += "-no-avx512";
            }

            if (BlasType == BlasType.OpenBlas)
            {
                path += "-openblas-";
                path += OpenBlasVendor.ToString();
            }

            /*
            if (EnableLinkTimeOptimization)
            {
                path += "-lto";
            }
            else
            {
                path += "-no-lto";
            }*/

            return path;
        }

        public OpenBlasVendor OpenBlasVendor { get; set; }

        public void Apply(ProcessArgumentBuilder processParameterBuilder)
        {
            processParameterBuilder.Append("-A ");
            processParameterBuilder.Append(Platform);

            processParameterBuilder.AddBlasType(this);

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

    public enum OpenBlasVendor
    {
        Generic,
        Acml,
        AcmlGpu,
        AcmlMp,
        Aocl,
        AoclMt,
        Arm,
        ArmIlp64,
        ArmIlp64Mp,
        ArmMp,
        Atlas,
        Intel1032,
        Intel1064Dyn,
        Intel1064Ilp,
        Intel1064IlpSeq,
        Intel1064Lp,
        Intel1064LpSeq,
        Nvhpc,
        OpenBlas,
        PhiPack,
        Scsl,
        ScslMp,
        Sgimath,
        SunPerf,
    }
}
