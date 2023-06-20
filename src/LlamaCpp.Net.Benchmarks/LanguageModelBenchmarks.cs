using BenchmarkDotNet.Attributes;

namespace LlamaCpp.Net.Benchmarks;

public class LanguageModelBenchmarks
{
    private readonly LanguageModel languageModel;

    public LanguageModelBenchmarks()
    {
        this.languageModel = new LanguageModel("models/117M", null);
        var fileName = "wizardLM-7B.ggmlv3.q4_0.bin";

        var modelPath = Path.Join(Constants.ModelDirectory, fileName);

        this.languageModel = new LanguageModel(modelPath, null);
    }

    [Benchmark]
    public void Tokenize()
    {
        var input = "This is a test";

        var tokens = this.languageModel.Tokenize(input);
    }

    [Benchmark]
    public void TokenToString()
    {
        var input = "This is a test";

        var tokens = this.languageModel.Tokenize(input);

        foreach (var token in tokens)
        {
            this.languageModel.TokenToString(token);
        }
    }

    [Benchmark]
    public void Infer()
    {
        var input = "This is a test";


        this.languageModel.Infer(input);
    }



}