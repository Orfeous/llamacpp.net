﻿using System;
using Cake.Core.IO;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Extensions
{
    public static class BlasExtensions
    {
        public static void AddBlasType(this ProcessArgumentBuilder processParameterBuilder, BuildSettings settings)
        {
            switch (settings.BlasType)
            {
                case BlasType.CuBlas:
                    processParameterBuilder.AppendCmakeOption("LLAMA_CUBLAS", true);
                    break;
                case BlasType.OpenBlas:
                    processParameterBuilder.AppendCmakeOption("LLAMA_BLAS", true);
                    processParameterBuilder.AppendCmakeOption("LLAMA_BLAS_VENDOR", settings.OpenBlasVendor.ToVendor());
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

        public static string ToVendor(this OpenBlasVendor vendor)
        {
            return vendor switch
            {
                OpenBlasVendor.Generic => "Generic",
                OpenBlasVendor.Acml => "ACML",
                OpenBlasVendor.AcmlGpu => "ACML_GPU",
                OpenBlasVendor.AcmlMp => "ACML_MP",
                OpenBlasVendor.Aocl => "AOCL",
                OpenBlasVendor.AoclMt => "AOCL_mt",
                OpenBlasVendor.Arm => "Arm",
                OpenBlasVendor.ArmIlp64 => "Arm_ilp64",
                OpenBlasVendor.ArmIlp64Mp => "Arm_ilp64_mp",
                OpenBlasVendor.ArmMp => "Arm_mp",
                OpenBlasVendor.Atlas => "ATLAS",
                OpenBlasVendor.Intel1032 => "Intel10_32",
                OpenBlasVendor.Intel1064Dyn => "Intel10_64_dyn",
                OpenBlasVendor.Intel1064Ilp => "Intel10_64ilp",
                OpenBlasVendor.Intel1064IlpSeq => "Intel10_64ilp_seq",
                OpenBlasVendor.Intel1064Lp => "Intel10_64lp",
                OpenBlasVendor.Intel1064LpSeq => "Intel10_64lp_seq",
                OpenBlasVendor.Nvhpc => "NVHPC",
                OpenBlasVendor.OpenBlas => "OpenBLAS",
                OpenBlasVendor.PhiPack => "PhiPACK",
                OpenBlasVendor.Scsl => "SCSL",
                OpenBlasVendor.ScslMp => "SCSL_mp",
                OpenBlasVendor.Sgimath => "SGIMATH",
                OpenBlasVendor.SunPerf => "SunPerf",
                _ => throw new ArgumentOutOfRangeException()
            };
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
