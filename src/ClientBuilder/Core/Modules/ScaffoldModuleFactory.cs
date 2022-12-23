﻿using System;
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
    private readonly IOptions<ClientBuilderOptions> optionsAccessor;
    private readonly IEnumerable<IScaffoldModule> scaffoldModules;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleFactory"/> class.
    /// </summary>
    /// <param name="optionsAccessor"></param>
    /// <param name="scaffoldModules"></param>
    public ScaffoldModuleFactory(
        IOptions<ClientBuilderOptions> optionsAccessor,
        IEnumerable<IScaffoldModule> scaffoldModules)
    {
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
            scaffoldModule.SetSourceDirectory(this.optionsAccessor.Value.ContentRootPath);
            await scaffoldModule.SetupAsync();
            scaffoldModule.ValidateModule();
            scaffoldModule.Sync();
            modules.Add(scaffoldModule);
        }

        return modules;
    }
}