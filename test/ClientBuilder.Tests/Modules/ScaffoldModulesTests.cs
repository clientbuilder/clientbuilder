using System.IO;
using System.Threading.Tasks;
using ClientBuilder.Core.Modules;
using ClientBuilder.Exceptions;
using ClientBuilder.TestAssembly.Modules.EmptyTest;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Modules;

public class ScaffoldModulesTests
{
    [InlineData("", "")]
    [InlineData("Test Module", "")]
    [InlineData("", "test.client")]
    [Theory]
    public void ValidateModule_OnSimpleModuleWithInvalidData_ShouldValidateModuleCorrectly(string name, string clientId)
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>())
        {
            Name = name,
            ClientId = clientId
        };

        Assert.Throws<ClientBuilderException>(() => module.ValidateModule());
    }

    [Fact]
    public void AddFile_OnAdditionalFile_ShouldInsertTheNewFile()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        var filesCount = module.GetFiles().Count;
        module.AddFile(new ScaffoldModuleFile());
        filesCount.Should().Be(module.GetFiles().Count - 1);
    }

    [Fact]
    public void AddFolder_OnAdditionalFolder_ShouldInsertTheNewFolder()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        var foldersCount = module.GetFolders().Count;
        module.AddFolder(new ScaffoldModuleFolder());
        foldersCount.Should().Be(module.GetFolders().Count - 1);
    }

    [Fact]
    public async Task GetFile_OnProperInput_ShouldReturnsTheFile()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        module.SetSourceDirectory("NewFolder");
        await module.SetupAsync();

        var file = module.GetFile("file1");
        file
            .Name
            .Should()
            .Be("file1.json");
    }

    [Fact]
    public async Task GetFolder_OnProperInput_ShouldReturnsTheFile()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        module.SetSourceDirectory("NewFolder");
        await module.SetupAsync();

        var folder = module.GetFolder("folder1");
        folder
            .Name
            .Should()
            .Be("folder1");
    }

    [Fact]
    public async Task GetFile_OnNonExistingFileInput_ShouldReturnsTheFile()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        module.SetSourceDirectory("NewFolder");
        await module.SetupAsync();

        var file = module.GetFile("file2");
        Assert.Null(file);
    }

    [Fact]
    public async Task GetFolder_OnNonExistingFolderInput_ShouldReturnsTheFile()
    {
        var module = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        module.SetSourceDirectory("NewFolder");
        await module.SetupAsync();

        var folder = module.GetFolder("folder2");
        Assert.Null(folder);
    }

    [Fact]
    public async Task Sync_OnNormalModuleSync_ShouldCheckCorrectly()
    {
        var fileSystemManagerMock = new Mock<IFileSystemManager>();
        fileSystemManagerMock
            .Setup(x => x.IsFileExists(It.IsAny<string>()))
            .Returns(true);
        
        var module = new SimpleTestModule(fileSystemManagerMock.Object);
        module.SetSourceDirectory(Directory.GetCurrentDirectory());
        await module.SetupAsync();
        module.Sync();

        module
            .Generated
            .Should()
            .Be(false);
    }
    
    [Fact]
    public async Task Sync_OnEmptyModuleSync_ShouldCheckCorrectly()
    {
        var module = new EmptyTestModule(Mock.Of<IFileSystemManager>());
        module.SetSourceDirectory(Directory.GetCurrentDirectory());
        await module.SetupAsync();
        module.Sync();

        module
            .Generated
            .Should()
            .Be(false);
    }
}