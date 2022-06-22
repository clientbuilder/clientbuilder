using System.Collections.Generic;
using Castle.Core.Logging;
using ClientBuilder.Core.Modules;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClientBuilder.Tests.Fakes;

public class FileSystemManagerFake : FileSystemManager
{
    public FileSystemManagerFake()
        : base(Mock.Of<ILogger<FileSystemManager>>())
    {
        this.CreatedFiles = new Dictionary<string, string>();
        this.CreatedFolders = new List<string>();
    }
    
    public IDictionary<string, string> CreatedFiles { get; }
    
    public IList<string> CreatedFolders { get; }

    public override bool CreateFile(string filePath, string fileContent)
    {
        if (!this.IsFileExists(filePath))
        {
            this.CreatedFiles[filePath] = fileContent;
            return true;
        }

        return false;
    }

    public override bool CreateFolder(string folderPath)
    {
        if (!this.IsFolderExists(folderPath))
        {
            this.CreatedFolders.Add(folderPath);
            return true;
        }

        return false;
    }

    public override bool IsFileExists(string filePath) =>
        this.CreatedFiles.ContainsKey(filePath);

    public override bool IsFolderExists(string folderPath) =>
        this.CreatedFolders.Contains(folderPath);
}