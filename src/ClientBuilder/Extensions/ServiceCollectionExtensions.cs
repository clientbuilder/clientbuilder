using System;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Core.Scanning;
using ClientBuilder.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Client Builder modules and related services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddClientBuilder(
        this IServiceCollection services,
        Action<ClientBuilderOptions> optionsAction)
    {
        var options = new ClientBuilderOptions();
        optionsAction?.Invoke(options);
        services.AddOptions<ClientBuilderOptions>();
        services.PostConfigure(optionsAction);

        services.AddSingleton<IFileSystemManager, FileSystemManager>();
        services.AddScoped<IScaffoldModuleFactory, ScaffoldModuleFactory>();
        services.AddScoped<IScaffoldModuleRepository, ScaffoldModuleRepository>();
        services.AddScoped<IScaffoldModuleGenerator, ScaffoldModuleGenerator>();
        services.AddScoped<IAssemblyScanner, AssemblyScanner>();
        services.AddScoped<IDescriptionExtractor, DescriptionExtractor>();
        services.AddScoped<ISourceRepository, SourceRepository>();

        foreach (var modulesType in options.ModulesTypes)
        {
            services.AddScoped(typeof(IScaffoldModule), modulesType);
        }

        return services;
    }
}