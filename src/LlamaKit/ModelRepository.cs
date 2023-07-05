using ByteSizeLib;
using LlamaKit.Abstractions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace LlamaKit;

public class ModelRepository : IModelRepository
{
    private readonly string _modelDirectory;
    private readonly ILogger<ModelRepository> _logger;
    private readonly IFileProvider _fileProvider;

    public ModelRepository(ILogger<ModelRepository> logger, IFileProvider fileProvider)
    {
        this._logger = logger;
        _fileProvider = fileProvider;

        _modelDirectory = "";

        this._logger = logger;
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




        var fileInfo = _fileProvider.GetFileInfo(fileName);

        var byteSize = ByteSize.FromBytes(fileInfo.Length);


        if (fileInfo.PhysicalPath == null)
        {
            throw new FileNotFoundException("Physical path not found");
        }

        return new RepositoryModel(fileName, fileInfo.PhysicalPath, byteSize);
    }
}