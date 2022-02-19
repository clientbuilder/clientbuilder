using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using ClientBuilder.Tests.Fakes;
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
    public async Task GenerateAsync_OnSimpleModule_ShouldGenerateModulesFilesAndFolders()
    {
        var module = new SimpleTestModule(this.fileSystemManager);
        module.SetSourceDirectory(string.Empty);
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
            .Be(Path.Combine(Directory.GetCurrentDirectory(), "folder1"));

        this.fileSystemManager
            .CreatedFiles
            .Should()
            .HaveCount(1);

        this.fileSystemManager
            .CreatedFiles
            .First()
            .Key
            .Should()
            .Be(Path.Combine(Directory.GetCurrentDirectory(), "file1.json"));

        this.fileSystemManager
            .CreatedFiles
            .First()
            .Value
            .Replace("\r\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace(" ", string.Empty)
            .Should()
            .Be("{\"data\":\"SimpleData\"}");
    }

    private IScaffoldModuleGenerator GetSubject(IScaffoldModuleRepository repository)
    {
        return new ScaffoldModuleGenerator(repository, this.fileSystemManager);
    }
}