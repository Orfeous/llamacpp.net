using BenchmarkDotNet.Running;

namespace LlamaCpp.Net.Benchmarks
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}