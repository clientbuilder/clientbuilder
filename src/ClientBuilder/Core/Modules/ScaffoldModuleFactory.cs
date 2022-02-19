using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClientBuilder.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Core.Modules;

/// <inheritdoc />
public class ScaffoldModuleFactory : IScaffoldModuleFactory
{
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IServiceProvider serviceProvider;
    private readonly ClientBuilderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleFactory"/> class.
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <param name="optionsAccessor"></param>
    /// <param name="serviceProvider"></param>
    public ScaffoldModuleFactory(
        IWebHostEnvironment hostEnvironment,
        IOptions<ClientBuilderOptions> optionsAccessor,
        IServiceProvider serviceProvider)
    {
        this.hostEnvironment = hostEnvironment;
        this.serviceProvider = serviceProvider;
        this.options = optionsAccessor.Value;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ScaffoldModule>> BuildScaffoldModulesAsync()
    {
        var modules = new List<ScaffoldModule>();

        string sourceDirectory = Path.GetFullPath(
            Path.Combine(
                this.hostEnvironment.ContentRootPath,
                $@"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}"));

        var scaffoldModulesTypes = this.options.ModulesTypes;

        var scopedServiceProvider = this.serviceProvider.CreateScope().ServiceProvider;

        foreach (var scaffoldModulesType in scaffoldModulesTypes)
        {
            if (scopedServiceProvider.GetService(scaffoldModulesType) is ScaffoldModule moduleInstance)
            {
                moduleInstance.SetSourceDirectory(sourceDirectory);
                await moduleInstance.SetupAsync();
                moduleInstance.ValidateModule();
                moduleInstance.Sync();
                modules.Add(moduleInstance);
            }
        }

        return modules;
    }
}