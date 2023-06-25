using Cake.Common;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Extensions;
using System;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Llama
{
    public sealed class BuildLlamaTask : Abstractions.BaseCmakeBuildTask
    {
        public override void Run(BuildContext context)
        {
            foreach (var setting in context.MsvcBuildSettings)
            {
                Run(context, setting, context.LlamaDependency);
            }
        }


        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningOnWindows();
        }

        protected override void Configure(CmakeOptions options, BuildSettings buildSettings)
        {
            options.Options["BUILD_SHARED_LIBS"] = true;
            options.Options["LLAMA_ALL_WARNINGS"] = true;
            options.Options["LLAMA_ALL_WARNINGS_3RD_PARTY"] = true;
            options.Options["LLAMA_CUDA_DMMV_F16"] = true;
            options.Options["LLAMA_STANDALONE"] = buildSettings.Standalone;


            switch (buildSettings.BlasType)
            {
                case BlasType.CuBlas:
                    options.Options["LLAMA_CUBLAS"] = true;
                    break;
                case BlasType.OpenBlas:
                    options.Options["LLAMA_BLAS"] = true;
                    options.Options["LLAMA_BLAS_VENDOR"] = buildSettings.OpenBlasVendor.ToVendor();
                    break;
                case BlasType.ClBlast:
                    options.Options["LLAMA_CLBLAST"] = true;
                    break;
                case BlasType.Blis:
                    throw new NotSupportedException("BLIS is not supported at this time.");
                case BlasType.None:
                    break;
                case BlasType.CLBlast:
                case BlasType.OpenBlasIntel:
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildSettings.BlasType), "Unknown BLAS type");
            }


            if (buildSettings.EnableKQuants)
            {
                options.Options["LLAMA_K_QUANTS"] = true;
            }

            /*
            if (buildSettings.EnableLinkTimeOptimization)
            {
                options.Options["LLAMA_LTO"] = true;
            }
            */

            if (buildSettings.Avx512Support != Avx512Support.None)
            {
                options.Options["LLAMA_AVX512"] = true;

                if (buildSettings.Avx512Support.HasFlag(Avx512Support.Vbmi))
                {
                    options.Options["LLAMA_AVX512_VBMI"] = true;
                }

                if (buildSettings.Avx512Support.HasFlag(Avx512Support.Vnni))
                {
                    options.Options["LLAMA_AVX512_VNNI"] = true;
                }
            }
        }
    }
}