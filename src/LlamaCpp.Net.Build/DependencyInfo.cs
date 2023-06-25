using Cake.Core.IO;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build;

public class DependencyInfo
{
    public string Name { get; init; }
    public string RepositoryUrl { get; init; }
    public string DesiredCommit { get; init; }


    public DirectoryPath SourcePath { get; set; }
    public DirectoryPath BuildPath { get; set; }

    public DirectoryPath GetOutputDirectory(BuildSettings setting)
    {
        return BuildPath.Combine(setting.BuildPath);
    }
}