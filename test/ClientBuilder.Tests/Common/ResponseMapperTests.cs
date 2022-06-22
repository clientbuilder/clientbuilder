using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Models;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Common;

public class ResponseMapperTests
{
    [Fact]
    public async Task MapToModel_OnScaffoldModule_ShouldMapToProperResponseModel()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        module.SetSourceDirectory("NewFolder");
        await module.SetupAsync();

        var mappedModule = ResponseMapper.MapToModel(module);

        mappedModule
            .Should()
            .BeEquivalentTo(new
            {
                module.Id,
                module.Name,
                module.Order,
                module.IconUrl,
                module.ScaffoldTypeName,
                module.SourceDirectory,
                module.Type,
                module.Generated,
                module.ClientId,
            });
        
        var expectedFiles = module.GetFiles().Select(x => new ScaffoldModuleFileSystemItemModel
        {
            Name = x.Name,
            Path = Path.Combine(x.RelativePath, x.Name),
        });

        var expectedFolders = module.GetFolders().Select(x => new ScaffoldModuleFileSystemItemModel
        {
            Name = x.Name,
            Path = Path.Combine(x.RelativePath, x.Name),
        });

        mappedModule
            .Files
            .Should()
            .HaveCount(1);

        var expectedFile = expectedFiles.First();
        var expectedFolder = expectedFolders.First();
        
        mappedModule
            .Files
            .First()
            .Should()
            .BeEquivalentTo(new
            {
                expectedFile.Name,
                expectedFile.Path,
            });
        
        mappedModule
            .Folders
            .First()
            .Should()
            .BeEquivalentTo(new
            {
                expectedFolder.Name,
                expectedFolder.Path,
            });
    }
}