using Cake.Core.IO;
using LlamaCpp.Net.Build.Extensions;
using System;

namespace LlamaCpp.Net.Build.Configuration
{
    public abstract record BuildSettings
    {
        public string? FriendlyName { get; init; }

        public DirectoryPath BuildPath => GetDirectoryPath();
        public BlasType BlasType { get; init; }

        public bool Standalone { get; init; }
        public bool EnableKQuants { get; init; }

        public Avx512Support Avx512Support { get; init; } = Avx512Support.None;

        public bool EnableLinkTimeOptimization { get; set; }

        public abstract string CompilerType { get; }
        public string BuildConfiguration { get; set; } = "Release";
        public string Triplet { get; set; } = "x64-windows";
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

        public virtual DirectoryPath GetDirectoryPath()

        {
            if (!string.IsNullOrWhiteSpace(FriendlyName))
            {
                return new DirectoryPath("build-" + CompilerType + "-" + FriendlyName);
            }

            var path = PackageName();

            var directoryPath = new DirectoryPath(path).Combine(Triplet);

            return directoryPath;
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

    }
}
