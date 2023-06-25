using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using System;
using System.IO;

namespace LlamaCpp.Net.Build.Tasks
{

    [TaskDescription("A friendly entrypoint for the build system")]
    public class DefaultTask : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
            var filePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "DefaultMessage.txt");
            var message = File.ReadAllText(filePath);

            context.Log.Information(message);
        }
    }
}