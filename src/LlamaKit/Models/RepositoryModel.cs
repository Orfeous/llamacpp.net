using ByteSizeLib;

namespace LlamaKit.Models;

public record RepositoryModel(string Name, string Path, ByteSize Size);