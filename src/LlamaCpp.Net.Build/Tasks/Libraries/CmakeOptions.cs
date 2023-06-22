using Cake.Core;
using Cake.Core.IO;
using System.Collections.Generic;

namespace LlamaCpp.Net.Build.Tasks.Libraries;

public class CmakeOptions
{
    public string? Generator { get; set; }
    public string? SourcePath { get; set; } = null!;
    public string? BuildPath { get; set; } = null!;
    public string? Triplet { get; set; }

    public Dictionary<string, object> Options { get; } = new Dictionary<string, object>();

    public CmakeOptions()
    {
    }


    public string Render()
    {
        var processParameterBuilder = new ProcessArgumentBuilder();
        if (!string.IsNullOrEmpty(Generator))
        {
            processParameterBuilder.Append("-G ");
            processParameterBuilder.AppendQuoted(Generator);
        }


        processParameterBuilder.Append("-S ");
        processParameterBuilder.AppendQuoted(SourcePath);
        processParameterBuilder.Append("-B ");
        processParameterBuilder.AppendQuoted(BuildPath);

        if (!string.IsNullOrEmpty(Triplet))
        {
            processParameterBuilder.Append("-DVCPKG_TARGET_TRIPLET=" + Triplet);
            processParameterBuilder.Append("-DVCPKG_HOST_TRIPLET=" + Triplet);


        }

        foreach (var pair in Options)
        {
            if (pair.Value is bool b)
            {
                var optionValue = b ? "ON" : "OFF";
                processParameterBuilder.Append($"-D{pair.Key}={optionValue}");
            }

            if (pair.Value is string s)
            {
                processParameterBuilder.Append($"-D{pair.Key}={s}");
            }
        }

        return processParameterBuilder.Render();
    }
}