using System.Text.RegularExpressions;
using ByteSizeLib;
using LlamaKit.Abstractions;
using LlamaKit.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace LlamaKit.Repositories;

public class ModelRepository : IModelRepository
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<ModelRepository> _logger;
    private readonly string _modelDirectory;

    public ModelRepository(ILogger<ModelRepository> logger, IFileProvider fileProvider)
    {
        _logger = logger;
        _fileProvider = fileProvider;

        _modelDirectory = "";

        _logger = logger;
    }

    public bool Exists(string fileName)
    {
        _logger.LogInformation($"Checking if model file {fileName} exists");
        return _fileProvider.GetFileInfo(fileName).Exists;
    }

    public RepositoryModel GetByFilename(string fileName)
    {
        return GetRepositoryModel(fileName);
    }

    public IEnumerable<RepositoryModel> ToList()
    {
        _logger.LogInformation($"Listing model files in {_modelDirectory}");


        foreach (var file in _fileProvider.GetDirectoryContents(_modelDirectory))
        {
            if (file.IsDirectory)
            {
                _logger.LogTrace($"Skipping directory {file.Name}");
                continue;
            }

            if (file.PhysicalPath == null)
            {
                _logger.LogWarning($"Physical path for {file.Name} not found");
                continue;
            }

            _logger.LogInformation($"Found model file {file.Name} at {file.PhysicalPath}");


            yield return GetRepositoryModel(file.Name);
        }
    }

    private RepositoryModel GetRepositoryModel(string fileName)
    {
        _logger.LogTrace($"Getting path for model file {fileName}");


        if (!Exists(fileName))
        {
            throw new FileNotFoundException($"Model file {fileName} not found");
        }

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);


        var fileInfo = _fileProvider.GetFileInfo(fileName);


        var physicalPath = fileInfo.PhysicalPath;
        if (physicalPath == null)
        {
            throw new FileNotFoundException("Physical path not found");
        }

        var byteSize = ByteSize.FromBytes(fileInfo.Length);

        // check magic number

        var magicNumber = GetMagicNumber(physicalPath);

        _logger.LogTrace($"Magic number for {fileName} is {magicNumber}");


        // if the filename contains -7B-, -30B- or similar, get the number.

        var match = Regex.Match(fileName, @"-(\d+)B");
        var modelSize = -1;
        if (match.Success)
        {
            var size = match.Groups[1].Value;

            if (int.TryParse(size, out var parsedSize))
            {
                modelSize = parsedSize;
            }
        }


        var family = ModelFamily.Unknown;

        if (fileNameWithoutExtension.Contains("Wizard", StringComparison.OrdinalIgnoreCase) &&
            fileNameWithoutExtension.Contains("Vicuna", StringComparison.OrdinalIgnoreCase))
        {
            family = ModelFamily.WizardVicuna;
        }
        else if (fileNameWithoutExtension.Contains("Wizard", StringComparison.OrdinalIgnoreCase))
        {
            family = ModelFamily.Wizard;
        }

        return new RepositoryModel
        {
            Name = fileNameWithoutExtension,
            Size = byteSize,
            Type = magicNumber,
            Path = physicalPath,
            ModelSize = modelSize,
            Family = family
        };
    }

    private SupportedModelTypes GetMagicNumber(string physicalPath)
    {
        using var file = File.OpenRead(physicalPath);
        using var reader = new BinaryReader(file);

        reader.BaseStream.Seek(0, SeekOrigin.Begin);

        var magic = reader.ReadUInt32();


        if (Enum.TryParse<SupportedModelTypes>(magic.ToString(), out var modelType))
        {
            _logger.LogTrace($"Model type {modelType} found for {physicalPath}");

            return modelType;
        }


        throw new NotSupportedException($"Model type {magic} not supported");
    }
}