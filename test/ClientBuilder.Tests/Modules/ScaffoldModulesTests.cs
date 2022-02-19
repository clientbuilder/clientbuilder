using ClientBuilder.Core.Modules;
using ClientBuilder.Exceptions;
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
}