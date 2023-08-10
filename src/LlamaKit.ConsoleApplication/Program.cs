using LlamaKit.Configuration;
using LlamaKit.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace LlamaKit.ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Join(AppContext.BaseDirectory, "logs", "log.txt"))
                .MinimumLevel.Verbose()
                .CreateLogger();

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var c = context.Configuration.GetSection(LlamaKitOptions.LlamaKit).Get<LlamaKitOptions>();
                    services.AddSerilog();


                    services.AddLogging();

                    services.AddSingleton<IFileProvider>(provider => new PhysicalFileProvider(c.ModelDirectory));

                    services.AddLlama(context.Configuration);

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}