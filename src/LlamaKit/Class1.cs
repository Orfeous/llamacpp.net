using ByteSizeLib;
using LlamaCpp.Net;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Samplers.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LlamaKit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLlama(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LlamaKitOptions>(c => configuration.GetSection(LlamaKitOptions.LlamaKit).Bind(c));

            services.AddSingleton<ILanguageModelFactory, LanguageModelFactory>();
            services.AddSingleton<IModelRepository, ModelRepository>();

            return services;
        }
    }


    public class LlamaKitOptions
    {
        public const string LlamaKit = "LlamaKit";
        public string? ModelDirectory { get; set; }
    }


    public class LanguageModelFactory : ILanguageModelFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IModelRepository _modelRepository;
        private readonly ILogger<LanguageModelFactory> _logger;

        public LanguageModelFactory(ILogger<LanguageModelFactory> logger,
            ILoggerFactory loggerFactory, IModelRepository modelRepository)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _modelRepository = modelRepository;
        }

        public ILanguageModel CreateModel(string fileName,
            LanguageModelOptions? options = null, Action<ISamplingPipelineBuilder>? builder = null)
        {
            if (_modelRepository.Exists(fileName))
            {
                _logger.LogInformation($"Loading model file {fileName}");
                var path = _modelRepository.GetByFilename(fileName);
                return new LanguageModel(path.Path, new Logger<LanguageModel>(_loggerFactory), options, builder);
            }

            else
            {
                throw new FileNotFoundException($"Model file {fileName} not found");
            }
        }
    }

    public interface IModelRepository
    {
        bool Exists(string fileName);
        RepositoryModel GetByFilename(string fileName);
        IEnumerable<RepositoryModel> ToList();
    }

    public class ModelRepository : IModelRepository
    {
        private readonly string _modelDirectory;
        private readonly ILogger<ModelRepository> _logger;

        public ModelRepository(ILogger<ModelRepository> logger, IOptions<LlamaKitOptions> options)
        {
            this._logger = logger;

            this._modelDirectory = options.Value.ModelDirectory;

            this._logger = logger;
            if (!Directory.Exists(_modelDirectory))
            {
                _logger.LogInformation($"Creating model directory {_modelDirectory}");
                Directory.CreateDirectory(_modelDirectory);
            }

            else
            {
                _logger.LogInformation($"Model directory {_modelDirectory} exists");
            }
        }

        public bool Exists(string fileName)
        {
            _logger.LogInformation($"Checking if model file {fileName} exists");
            return File.Exists(Path.Join(_modelDirectory, fileName));
        }

        public RepositoryModel GetByFilename(string fileName)
        {
            return GetRepositoryModel(fileName);
        }

        public IEnumerable<RepositoryModel> ToList()
        {
            _logger.LogInformation($"Listing model files in {_modelDirectory}");
            foreach (var file in Directory.EnumerateFiles(_modelDirectory))
            {
                _logger.LogInformation($"Found model file {file}");

                var fileName = Path.GetFileName(file);
                yield return GetRepositoryModel(fileName);
            }
        }

        private RepositoryModel GetRepositoryModel(string fileName)
        {
            _logger.LogInformation($"Getting path for model file {fileName}");

            if (!Exists(fileName))
            {
                throw new FileNotFoundException($"Model file {fileName} not found");
            }

            var file = Path.Join(_modelDirectory, fileName);

            var fileInfo = new FileInfo(file);

            var byteSize = ByteSize.FromBytes(fileInfo.Length);


            return new RepositoryModel(Path.GetFileName(file), file, byteSize);
        }
    }

    public record RepositoryModel(string Name, string Path, ByteSize Size);

    public interface ILanguageModelFactory
    {
        public ILanguageModel CreateModel(string fileName, LanguageModelOptions? options = null, Action<ISamplingPipelineBuilder>? builder = null
            );
    }
}