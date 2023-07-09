using LlamaKit.Models;
using LlamaKit.Repositories;

namespace LlamaKit.Abstractions;

public interface IPresetRepository
{
    IEnumerable<PresetModel> GetApplicablePresets(string modelName, ModelFamily modelFamily, int modelModelSize);
}