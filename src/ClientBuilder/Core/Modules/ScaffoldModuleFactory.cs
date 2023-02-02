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
    private readonly IFileSystemManager fileSystemManager;
    private readonly IOptions<ClientBuilderOptions> optionsAccessor;
    private readonly IEnumerable<IScaffoldModule> scaffoldModules;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleFactory"/> class.
    /// </summary>
    /// <param name="fileSystemManager"></param>
    /// <param name="optionsAccessor"></param>
    /// <param name="scaffoldModules"></param>
    public ScaffoldModuleFactory(
        IFileSystemManager fileSystemManager,
        IOptions<ClientBuilderOptions> optionsAccessor,
        IEnumerable<IScaffoldModule> scaffoldModules)
    {
        this.fileSystemManager = fileSystemManager;
        this.optionsAccessor = optionsAccessor;
        this.scaffoldModules = scaffoldModules;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ScaffoldModule>> BuildScaffoldModulesAsync()
    {
        var modules = new List<ScaffoldModule>();
        foreach (var scaffoldModuleInstance in this.scaffoldModules)
        {
            var scaffoldModule = (ScaffoldModule)scaffoldModuleInstance;
            await scaffoldModule.SetupAsync();
            scaffoldModule.ValidateModule();
            scaffoldModule.ConsolidateModule(this.optionsAccessor.Value);
            scaffoldModule.Sync(this.fileSystemManager);
            modules.Add(scaffoldModule);
        }

        return modules;
    }
}