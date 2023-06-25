namespace LlamaCpp.Net.Build.Configuration;

public record ClangBuildSettings : BuildSettings
{
    public override string CompilerType { get; } = "generic";
}