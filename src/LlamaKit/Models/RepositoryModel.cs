using ByteSizeLib;

namespace LlamaKit;

public record RepositoryModel(string Name, string Path, ByteSize Size);