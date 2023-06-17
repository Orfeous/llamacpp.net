using Cake.Core;
using Cake.Core.IO;

namespace LlamaCpp.Net.Build.Extensions
{
    public static class ProcessParameterBuilderExtensions
    {
        public static void AppendCmakeOption(this ProcessArgumentBuilder processParameterBuilder, string optionName,
            string optionValue)
        {
            processParameterBuilder.Append($"-D{optionName}={optionValue}");
        }

        public static void AppendCmakeOption(this ProcessArgumentBuilder processParameterBuilder, string optionName,
            bool value)
        {
            var optionValue = value ? "ON" : "OFF";
            processParameterBuilder.Append($"-D{optionName}={optionValue}");
        }
    }
}
