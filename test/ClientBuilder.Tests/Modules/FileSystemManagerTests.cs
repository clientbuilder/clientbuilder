using System;
using ClientBuilder.Core.Modules;
using FluentAssertions;
using Xunit;

namespace ClientBuilder.Tests.Modules;

public class FileSystemManagerTests
{
    [Fact]
    public void CreateFile_OnNoInput_ShouldThrows()
    {
        var manager = this.GetSubject();
        var exception = Assert.Throws<ArgumentNullException>(() => manager.CreateFile(null, null));
        exception
            .ParamName
            .Should()
            .Be("filePath");
    }
    
    [Fact]
    public void CreateFolder_OnNoInput_ShouldThrows()
    {
        var manager = this.GetSubject();
        var exception = Assert.Throws<ArgumentNullException>(() => manager.CreateFolder(null));
        exception
            .ParamName
            .Should()
            .Be("folderPath");
    }
    
    private IFileSystemManager GetSubject()
    {
        return new FileSystemManager();
    }
}