using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClientBuilder.Common;
using ClientBuilder.Models;

namespace ClientBuilder.Core;

/// <inheritdoc />
public class ScaffoldModuleGenerator : IScaffoldModuleGenerator
{
    private readonly IScaffoldModuleRepository scaffoldModuleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleGenerator"/> class.
    /// </summary>
    /// <param name="scaffoldModuleRepository"></param>
    public ScaffoldModuleGenerator(IScaffoldModuleRepository scaffoldModuleRepository)
    {
        this.scaffoldModuleRepository = scaffoldModuleRepository;
    }

    /// <inheritdoc/>
    public GenerationResult Generate(IEnumerable<string> modulesIds)
    {
        var targetModules = this.scaffoldModuleRepository.GetModules().Where(x => modulesIds.Contains(x.Id));
        return this.GenerateModules(targetModules);
    }

    /// <inheritdoc/>
    public GenerationResult Generate(InstanceType instanceType)
    {
        var targetModules = this.scaffoldModuleRepository.GetModulesByInstance(instanceType);
        return this.GenerateModules(targetModules);
    }

    /// <inheritdoc/>
    public GenerationResult Generate(string clientId)
    {
        var targetModules = this.scaffoldModuleRepository.GetModulesByClientId(clientId);
        return this.GenerateModules(targetModules);
    }

    private GenerationResult GenerateModules(IEnumerable<ScaffoldModule> modules)
    {
        var generationResults = new List<GenerationResult>();
        foreach (var module in modules)
        {
            var result = this.GenerateModule(module, out var errors);
            generationResults.Add(new GenerationResult
            {
                GenerationStatus = result,
                Errors = errors,
            });
        }

        return this.MergeGenerationResults(generationResults);
    }

    private GenerationResult MergeGenerationResults(IEnumerable<GenerationResult> results)
    {
        var errorsList = new List<string>();
        var status = ScaffoldModuleGenerationStatusType.SuccessfulWithErrors;
        if (results.All(x => x.GenerationStatus == ScaffoldModuleGenerationStatusType.Successful))
        {
            status = ScaffoldModuleGenerationStatusType.Successful;
        }
        else if (results.All(x => x.GenerationStatus == ScaffoldModuleGenerationStatusType.Unsuccessful))
        {
            status = ScaffoldModuleGenerationStatusType.Unsuccessful;
        }

        foreach (var result in results)
        {
            errorsList.AddRange(result.Errors);
        }

        return new GenerationResult
        {
            GenerationStatus = status,
            Errors = errorsList,
        };
    }

    private ScaffoldModuleGenerationStatusType GenerateModule(ScaffoldModule module, out IEnumerable<string> errorMessages)
    {
        var errorMessagesList = new List<string>();
        try
        {
            var folders = module.GetFolders();
            foreach (var folder in folders)
            {
                try
                {
                    var folderPath = Path.Combine(module.SourceDirectory, folder.RelativePath, folder.Name);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }
                catch (Exception ex)
                {
                    errorMessagesList.Add(ex.Message);
                }
            }

            var files = module.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    string fileContent = file.BuildContent();
                    string filePath = Path.Combine(module.SourceDirectory, file.RelativePath, file.Name);
                    if (!File.Exists(filePath) || module.Locked)
                    {
                        File.WriteAllText(filePath, fileContent);
                    }
                }
                catch (Exception ex)
                {
                    errorMessagesList.Add(ex.Message);
                }
            }

            var expectedGeneratedItemsCount = folders.Count + files.Count;

            var generationResult = ScaffoldModuleGenerationStatusType.Unsuccessful;
            if (!errorMessagesList.Any())
            {
                generationResult = ScaffoldModuleGenerationStatusType.Successful;
            }
            else if (errorMessagesList.Count < expectedGeneratedItemsCount)
            {
                generationResult = ScaffoldModuleGenerationStatusType.SuccessfulWithErrors;
            }
            else
            {
                generationResult = ScaffoldModuleGenerationStatusType.Unsuccessful;
            }

            errorMessages = errorMessagesList;
            return generationResult;
        }
        catch (Exception ex)
        {
            errorMessages = new List<string> { ex.Message };
            return ScaffoldModuleGenerationStatusType.Unsuccessful;
        }
    }
}