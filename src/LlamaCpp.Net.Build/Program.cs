using Cake.Frosting;
using System;

namespace LlamaCpp.Net.Build
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return new CakeHost()
                .UseContext<BuildContext>()
                .InstallTool(new Uri("dotnet:?package=GitVersion.Tool"))
                .Run(args);
        }
    }
}
