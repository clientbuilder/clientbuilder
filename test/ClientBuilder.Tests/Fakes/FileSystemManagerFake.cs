using System.Collections.Generic;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.Tests.Fakes;

public class FileSystemManagerFake : FileSystemManager
{
    public FileSystemManagerFake()
    {
        this.CreatedFiles = new Dictionary<string, string>();
        this.CreatedFolders = new List<string>();
    }
    
    public IDictionary<string, string> CreatedFiles { get; }
    
    public IList<string> CreatedFolders { get; }

    public override void CreateFile(string filePath, string fileContent)
    {
        if (!this.IsFileExists(filePath))
        {
            this.CreatedFiles[filePath] = fileContent;
        }
    }

    public override void CreateFolder(string folderPath)
    {
        if (!this.IsFolderExists(folderPath))
        {
            this.CreatedFolders.Add(folderPath);
        }
    }

    public override bool IsFileExists(string filePath) =>
        this.CreatedFiles.ContainsKey(filePath);

    public override bool IsFolderExists(string folderPath) =>
        this.CreatedFolders.Contains(folderPath);
}