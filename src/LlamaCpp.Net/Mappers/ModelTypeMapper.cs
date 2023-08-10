using LlamaCpp.Net.Native;
using Riok.Mapperly.Abstractions;

namespace LlamaCpp.Net;

/// <summary>
/// </summary>
[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByValue)]
public static partial class ModelTypeMapper
{
    internal static partial LLamaFtype ToFType(ModelType model);
}