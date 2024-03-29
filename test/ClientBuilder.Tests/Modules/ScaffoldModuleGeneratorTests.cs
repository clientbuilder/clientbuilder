﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using ClientBuilder.TestAssembly.Modules.TestWithError;
using ClientBuilder.TestAssembly.Modules.TestWithPartialError;
using ClientBuilder.Tests.Fakes;
using ClientBuilder.Tests.Shared;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Modules;

public class ScaffoldModuleGeneratorTests
{
    private readonly FileSystemManagerFake fileSystemManager;

    public ScaffoldModuleGeneratorTests()
    {
        this.fileSystemManager = new FileSystemManagerFake();
    }

    [Fact]
    public async Task GenerateAsync_OnModuleWithPartialError_ShouldGenerateNothing()
    {
        var module = new TestWithPartialErrorModule();
        await module.SetupAsync();
        module.ConsolidateModule(new OptionsAccessorFake().Value);
        
        var modules = (new List<ScaffoldModule> { module }).AsReadOnly();
        
        var repositoryMock = new Mock<ScaffoldModuleRepository>(null);
        repositoryMock
            .Setup(x => x.GetModulesAsync())
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(modules));

        var generator = this.GetSubject(repositoryMock.Object);

        var generationResult = await generator.GenerateAsync(new List<string> { module.Id });
        
        generationResult
            .Errors
            .Should()
            .HaveCount(1);
        
        generationResult
            .GenerationStatus
            .Should()
            .Be(ScaffoldModuleGenerationStatusType.SuccessfulWithErrors);
        
        this.fileSystemManager
            .CreatedFolders
            .Should()
            .HaveCount(1);

        this.fileSystemManager
            .CreatedFiles
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public async Task GenerateAsync_OnModuleWithError_ShouldGenerateNothing()
    {
        var module = new TestWithErrorModule();
        await module.SetupAsync();
        
        var modules = (new List<ScaffoldModule> { module }).AsReadOnly();
        
        var repositoryMock = new Mock<ScaffoldModuleRepository>(null);
        repositoryMock
            .Setup(x => x.GetModulesAsync())
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(modules));

        var generator = this.GetSubject(repositoryMock.Object);

        var generationResult = await generator.GenerateAsync(new List<string> { module.Id });
        
        generationResult
            .Errors
            .Should()
            .HaveCount(1);
        
        generationResult
            .GenerationStatus
            .Should()
            .Be(ScaffoldModuleGenerationStatusType.Unsuccessful);
        
        this.fileSystemManager
            .CreatedFolders
            .Should()
            .HaveCount(0);

        this.fileSystemManager
            .CreatedFiles
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public async Task GenerateAsync_OnSimpleModule_ShouldGenerateModulesFilesAndFolders()
    {
        var module = new SimpleTestModule();
        await module.SetupAsync();
        var options = new OptionsAccessorFake().Value;
        module.ConsolidateModule(options);
        
        var modules = (new List<ScaffoldModule> { module }).AsReadOnly();
        
        var repositoryMock = new Mock<ScaffoldModuleRepository>(null);
        repositoryMock
            .Setup(x => x.GetModulesAsync())
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(modules));

        var generator = this.GetSubject(repositoryMock.Object);

        var generationResult = await generator.GenerateAsync(new List<string> { module.Id });
        
        generationResult
            .Errors
            .Should()
            .BeEmpty();
        
        generationResult
            .GenerationStatus
            .Should()
            .Be(ScaffoldModuleGenerationStatusType.Successful);
        
        this.fileSystemManager
            .CreatedFolders
            .Should()
            .HaveCount(1);
        
        this.fileSystemManager
            .CreatedFolders
            .First()
            .Should()
            .Be(Path.Combine(options.ContentRootPath, OptionsAccessorFake.ExpectedClientPath, "folder1"));

        this.fileSystemManager
            .CreatedFiles
            .Should()
            .HaveCount(1);

        this.fileSystemManager
            .CreatedFiles
            .First()
            .Key
            .Should()
            .Be(Path.Combine(options.ContentRootPath, OptionsAccessorFake.ExpectedClientPath, "folder1", "file1.json"));

        TestUtilities
            .NormalizeJson(
                this.fileSystemManager
                .CreatedFiles
                .First()
                .Value)
            .Should()
            .Be(TestUtilities.NormalizeJson("{\"data\":\"SimpleData\"}"));
    }

    [Fact]
    public async Task GenerateAsync_OnSimpleModuleByClientId_ShouldGenerateModulesFilesAndFolders()
    {
        var module = new SimpleTestModule();
        await module.SetupAsync();
        var options = new OptionsAccessorFake().Value;
        module.ConsolidateModule(options);
        
        var modules = (new List<ScaffoldModule> { module }).AsReadOnly();
        
        var repositoryMock = new Mock<ScaffoldModuleRepository>(null);
        repositoryMock
            .Setup(x => x.GetModulesByClientIdAsync("test.client"))
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(modules));

        var generator = this.GetSubject(repositoryMock.Object);

        var generationResult = await generator.GenerateAsync("test.client");
        
        generationResult
            .Errors
            .Should()
            .BeEmpty();
        
        generationResult
            .GenerationStatus
            .Should()
            .Be(ScaffoldModuleGenerationStatusType.Successful);
        
        this.fileSystemManager
            .CreatedFolders
            .Should()
            .HaveCount(1);
        
        this.fileSystemManager
            .CreatedFolders
            .First()
            .Should()
            .Be(Path.Combine(options.ContentRootPath, OptionsAccessorFake.ExpectedClientPath, "folder1"));

        this.fileSystemManager
            .CreatedFiles
            .Should()
            .HaveCount(1);

        this.fileSystemManager
            .CreatedFiles
            .First()
            .Key
            .Should()
            .Be(Path.Combine(options.ContentRootPath, OptionsAccessorFake.ExpectedClientPath, "folder1", "file1.json"));

        TestUtilities
            .NormalizeJson(
                this.fileSystemManager
                    .CreatedFiles
                    .First()
                    .Value)
            .Should()
            .Be(TestUtilities.NormalizeJson("{\"data\":\"SimpleData\"}"));
    }
    
    private IScaffoldModuleGenerator GetSubject(IScaffoldModuleRepository repository)
    {
        return new ScaffoldModuleGenerator(repository, this.fileSystemManager);
    }
}