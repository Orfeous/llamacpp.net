using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.GitVersion;

namespace LlamaCpp.Net.Build.Tasks.Dotnet;

public static class DotNetSettingsExtensions
{
    public static DotNetMSBuildSettings AppendVersionArguments(this DotNetMSBuildSettings settings,
        GitVersion version)
    {
        return settings.SetVersion(version.LegacySemVerPadded)
            .SetAssemblyVersion(version.MajorMinorPatch)
            .SetFileVersion(version.MajorMinorPatch)
            .SetInformationalVersion(version.InformationalVersion);
    }
}