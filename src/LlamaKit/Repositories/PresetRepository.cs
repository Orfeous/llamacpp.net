using LlamaKit.Abstractions;
using LlamaKit.Models;
using Microsoft.Extensions.Logging;

namespace LlamaKit.Repositories;

public class PresetRepository : IPresetRepository
{
    private readonly ILogger _logger;
    private readonly List<PresetModel> _presets;

    public PresetRepository(ILogger<PresetRepository> logger)
    {
        _logger = logger;
        _presets = new List<PresetModel>
        {
            new PresetModel
            {
                Family = ModelFamily.Wizard,
                Size = 7,
                PromptSuffix = "\n\n### Response:"
            },
            new PresetModel
            {
                Family = ModelFamily.WizardVicuna,
                Size = 13,
                InitialPrompt =
                    "A chat between a curious user and an artificial intelligence assistant. The assistant gives helpful, detailed, and polite answers to the user's questions.  \r\n",
                PromptPrefix = "USER: ",
                PromptSuffix = "\r\nASSISTANT:"
            },
            new PresetModel
            {
                Family = ModelFamily.WizardVicuna,
                Size = 30,
                InitialPrompt =
                    "A chat between a curious user and an artificial intelligence assistant. The assistant gives helpful, detailed, and polite answers to the user's questions. USER: hello, who are you? ASSISTANT: \r\n"
            }
        };
    }

    public IEnumerable<PresetModel> GetApplicablePresets(string modelName, ModelFamily modelFamily, int modelModelSize)
    {
        var list = new List<PresetModel>();

        var presets = _presets.Where(model => model.Family == modelFamily && model.Size == modelModelSize).ToList();

        if (!presets.Any())
        {
            list.Add(new PresetModel
            {
                Name = "Default"
            });
        }
        else
        {
            list.AddRange(presets);
        }


        _logger.LogInformation($"Found {presets.Count} presets for {modelName}.");

        return list.ToList();
    }
}