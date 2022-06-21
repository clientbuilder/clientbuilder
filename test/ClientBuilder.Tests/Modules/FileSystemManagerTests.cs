using System;
using System.IO;
using ClientBuilder.Core.Modules;
using ClientBuilder.Tests.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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
    public void CreateFile_WithCorrectInput_ShouldCreatesTheFile()
    {
        var loggerMock = new Mock<ILogger<FileSystemManager>>();
        var manager = this.GetSubject(loggerMock);
        manager.CreateFile("file.txt", "file content");

        loggerMock
            .VerifyDebugWasCalled("Client Builder has created a file (file.txt)");
        
        File.Delete("file.txt");
    }
    
    [Fact]
    public void IsFileExists_WithCorrectInput_ShouldBeInvokedWithoutAnError()
    {
        var loggerMock = new Mock<ILogger<FileSystemManager>>();
        var manager = this.GetSubject(loggerMock);
        manager.IsFileExists("file.txt");

        loggerMock
            .VerifyDebugWasCalled("Check for a file existence has been executed");
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
    
    [Fact]
    public void CreateFolder_WithCorrectInput_ShouldCreatesTheFolder()
    {
        var loggerMock = new Mock<ILogger<FileSystemManager>>();
        var manager = this.GetSubject(loggerMock);
        manager.CreateFolder("NewFolder");

        loggerMock
            .VerifyDebugWasCalled("Client Builder has created a folder (NewFolder)");
        
        Directory.Delete("NewFolder");
    }
    
    [Fact]
    public void IsFolderExists_WithCorrectInput_ShouldBeInvokedWithoutAnError()
    {
        var loggerMock = new Mock<ILogger<FileSystemManager>>();
        var manager = this.GetSubject(loggerMock);
        manager.IsFolderExists("NewFolder");

        loggerMock
            .VerifyDebugWasCalled("Check for a folder existence has been executed");
    }
    
    private IFileSystemManager GetSubject(Mock<ILogger<FileSystemManager>> loggerMock = null)
    {
        return new FileSystemManager(loggerMock?.Object ?? Mock.Of<ILogger<FileSystemManager>>());
    }
}