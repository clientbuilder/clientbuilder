using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Models;
using ClientBuilder.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for application builder.
/// </summary>
public static class ApplicationBuilderExtensions
{
    private const string BaseRoute = "/_cb";
    private const string ApiBaseRoute = $"{BaseRoute}/api/scaffold";

    /// <summary>
    /// Exposes Client Builder UI and API. Consider using that middleware only in development environment.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Design Requirement")]
    public static IEndpointRouteBuilder UseClientBuilder(this IEndpointRouteBuilder app)
    {
        app
            .MapGet(BaseRoute, () =>
            {
                var template = new ClientBuilderHtml()
                {
                    Session = new Dictionary<string, object>
                    {
                        { "Styles", ReadAssemblyFile("ClientBuilderStyles.css") },
                        { "Scripts", ReadAssemblyFile("ClientBuilderScripts.js") },
                    },
                };

                return Results.Text(template.TransformText(), "text/html");
            })
        .ExcludeFromDescription();

        app
            .MapGet($"{ApiBaseRoute}/modules", async (IScaffoldModuleRepository moduleRepository) =>
            {
                var modules = await moduleRepository.GetModulesAsync();
                return Results.Ok(modules.Select(ResponseMapper.MapToModel));
            })
            .ExcludeFromDescription();

        app
            .MapPost(
                $"{ApiBaseRoute}/generate",
                async (
                    GenerationByIdRequest request,
                    IScaffoldModuleRepository moduleRepository,
                    IScaffoldModuleGenerator moduleGenerator) =>
                {
                    var modulesForGeneration = new List<ScaffoldModule>();
                    if (string.IsNullOrWhiteSpace(request.ModuleId))
                    {
                        var modules = await moduleRepository.GetModulesAsync();
                        modulesForGeneration.AddRange(modules);
                    }
                    else
                    {
                        var targetModule = await moduleRepository.GetModuleAsync(request.ModuleId);
                        modulesForGeneration.Add(targetModule);
                    }

                    return await TriggerGenerationAsync(modulesForGeneration, moduleGenerator);
                })
            .ExcludeFromDescription();

        app
            .MapPost(
                $"{ApiBaseRoute}/generate/by-client",
                async (
                    GenerationByClientIdRequest request,
                    IScaffoldModuleRepository moduleRepository,
                    IScaffoldModuleGenerator moduleGenerator) =>
                {
                    var modules = await moduleRepository.GetModulesByClientIdAsync(request.ClientId);
                    return await TriggerGenerationAsync(modules, moduleGenerator);
                })
            .ExcludeFromDescription();

        return app;
    }

    private static string ReadAssemblyFile(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(x => x.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static async Task<IResult> TriggerGenerationAsync(
        IEnumerable<ScaffoldModule> modules,
        IScaffoldModuleGenerator moduleGenerator)
    {
        try
        {
            var result = await moduleGenerator.GenerateAsync(modules.Select(x => x.Id));
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}