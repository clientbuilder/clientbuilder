using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Models;

namespace ClientBuilder.Core.Modules;

/// <inheritdoc />
public class ScaffoldModuleGenerator : IScaffoldModuleGenerator
{
    private readonly IScaffoldModuleRepository scaffoldModuleRepository;
    private readonly IFileSystemManager fileSystemManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleGenerator"/> class.
    /// </summary>
    /// <param name="scaffoldModuleRepository"></param>
    /// <param name="fileSystemManager"></param>
    public ScaffoldModuleGenerator(
        IScaffoldModuleRepository scaffoldModuleRepository,
        IFileSystemManager fileSystemManager)
    {
        this.scaffoldModuleRepository = scaffoldModuleRepository;
        this.fileSystemManager = fileSystemManager;
    }

    /// <inheritdoc/>
    public async Task<GenerationResult> GenerateAsync(IEnumerable<string> modulesIds)
    {
        var targetModules = (await this.scaffoldModuleRepository.GetModulesAsync())
            .Where(x => modulesIds.Contains(x.Id));
        return this.GenerateModules(targetModules);
    }

    /// <inheritdoc/>
    public async Task<GenerationResult> GenerateAsync(string clientId)
    {
        var targetModules = await this.scaffoldModuleRepository.GetModulesByClientIdAsync(clientId);
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
            var generatedFoldersCount = 0;
            var folders = module.GetFolders();
            foreach (var folder in folders)
            {
                try
                {
                    var folderPath = Path.Combine(module.SourceDirectory, folder.RelativePath, folder.Name);
                    if (this.fileSystemManager.CreateFolder(folderPath))
                    {
                        generatedFoldersCount++;
                    }
                }
                catch (Exception ex)
                {
                    errorMessagesList.Add(ex.Message);
                }
            }

            var generatedFilesCount = 0;
            var files = module.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    string fileContent = file.BuildContent();
                    string filePath = Path.Combine(module.SourceDirectory, file.RelativePath, file.Name);
                    bool isFileExists = this.fileSystemManager.IsFileExists(filePath);
                    if (!isFileExists || file.Locked)
                    {
                        if (this.fileSystemManager.CreateFile(filePath, fileContent))
                        {
                            generatedFilesCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessagesList.Add(ex.Message);
                }
            }

            var generationResult = ScaffoldModuleGenerationStatusType.Unsuccessful;
            if (!errorMessagesList.Any())
            {
                generationResult = ScaffoldModuleGenerationStatusType.Successful;
            }
            else if (errorMessagesList.Any() && (generatedFilesCount + generatedFoldersCount) > 0)
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