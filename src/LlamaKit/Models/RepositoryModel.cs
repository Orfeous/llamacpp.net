using ByteSizeLib;

namespace LlamaKit.Models;

public record RepositoryModel
{
    public string Name { get; init; }
    public string Path { get; init; }
    public ByteSize Size { get; init; }
    public SupportedModelTypes Type { get; set; }
    public int ModelSize { get; set; }
    public ModelFamily Family { get; set; }
}