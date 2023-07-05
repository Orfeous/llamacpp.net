﻿using LlamaKit.Abstractions;
using LlamaKit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaKit.DependencyInjection
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
}