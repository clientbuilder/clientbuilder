using System;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Core.Scanning;
using ClientBuilder.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Registers all Client Builder modules and related services.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddClientBuilder(this WebApplicationBuilder builder, Action<ClientBuilderOptions> optionsAction)
    {
        if (!builder.Environment.IsDevelopment())
        {
            return builder;
        }

        var options = new ClientBuilderOptions();
        optionsAction?.Invoke(options);

        builder.Services.AddOptions<ClientBuilderOptions>();
        builder.Services.PostConfigure(optionsAction);

        builder.Services.AddSingleton<IFileSystemManager, FileSystemManager>();
        builder.Services.AddScoped<IScaffoldModuleFactory, ScaffoldModuleFactory>();
        builder.Services.AddScoped<IScaffoldModuleRepository, ScaffoldModuleRepository>();
        builder.Services.AddScoped<IScaffoldModuleGenerator, ScaffoldModuleGenerator>();
        builder.Services.AddScoped<IAssemblyScanner, AssemblyScanner>();
        builder.Services.AddScoped<IDescriptionExtractor, DescriptionExtractor>();
        builder.Services.AddScoped<ISourceRepository, SourceRepository>();

        foreach (var modulesType in options.ModulesTypes)
        {
            builder.Services.AddScoped(typeof(IScaffoldModule), modulesType);
        }

        builder.Services.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy(Constants.ClientBuilderCorsPolicy, corsBuilder =>
            {
                corsBuilder
                    .WithOrigins(Constants.ClientBuilderClientUrls)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }
}