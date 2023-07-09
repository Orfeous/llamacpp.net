using LlamaKit.Models;

namespace LlamaKit.Abstractions;

public interface IModelRepository
{
    bool Exists(string fileName);
    RepositoryModel GetByFilename(string fileName);
    IEnumerable<RepositoryModel> ToList();
}