using System;
using Cake.Core.IO;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Extensions
{
    public static class BlasExtensions
    {
        public static void AddBlasType(this ProcessArgumentBuilder processParameterBuilder, BlasType blasType)
        {
            switch (blasType)
            {
                case BlasType.CuBlas:
                    processParameterBuilder.AppendCmakeOption("LLAMA_CUBLAS", true);
                    break;
                case BlasType.OpenBlas:
                    processParameterBuilder.AppendCmakeOption("LLAMA_OPENBLAS", true);
                    break;
                case BlasType.OpenBlasIntel:
                    processParameterBuilder.AppendCmakeOption("LLAMA_OPENBLAS", true);
                    processParameterBuilder.AppendCmakeOption("LLAMA_BLAS_VENDOR", "Intel10_64lp");
                    break;
                case BlasType.ClBlast:
                    processParameterBuilder.AppendCmakeOption("LLAMA_CLBLAST", true);
                    break;
                case BlasType.Blis:
                    throw new NotSupportedException("BLIS is not supported at this time.");
                case BlasType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(blasType), "Unknown BLAS type");
            }
        }

        public static string AsString(this BlasType blasType)
        {
            return blasType switch
            {
                BlasType.CuBlas => "cuBLAS",
                BlasType.OpenBlas => "OpenBLAS",
                BlasType.OpenBlasIntel => "OpenBLAS-Intel",
                BlasType.ClBlast => "CLBlast",
                BlasType.Blis => "BLIS",
                _ => "NoBlas"
            };
        }
    }
}
