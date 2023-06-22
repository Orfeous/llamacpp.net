using Cake.Common;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Extensions;
using System;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Llama.Cmake
{



    [TaskName("Cmake.Msvc.Configure")]
    [IsDependentOn(typeof(CleanTask))]
    [IsDependentOn(typeof(CloneLlamaTask))]
    public sealed class ConfigureTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            foreach (var setting in context.MsvcBuildSettings)
            {
                context.Log.Information($"Configuring {setting.BuildPath.FullPath}");

                Run(context, setting);
            }
        }

        private static void Run(BuildContext context, MsvcBuildSettings setting)
        {
            var cmakeOptions = new CmakeOptions
            {
                Options =
                {
                    ["BUILD_SHARED_LIBS"] = true,
                    ["LLAMA_ALL_WARNINGS"] = true,
                    ["LLAMA_ALL_WARNINGS_3RD_PARTY"] = true,
                    ["LLAMA_CUDA_DMMV_F16"] = true

                },
                SourcePath = context.LlamaDependency.SourcePath.FullPath,
                BuildPath = context.LlamaDependency.GetOutputDirectory(setting).FullPath,
                Generator = context.MsvcGenerator,
            };


            Apply(cmakeOptions, setting);

            var arguments = cmakeOptions.Render();
            context.StartProcess("cmake", new ProcessSettings { Arguments = arguments });
        }

        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningOnWindows();
        }

        private static void Apply(CmakeOptions options, MsvcBuildSettings buildSettings)
        {
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