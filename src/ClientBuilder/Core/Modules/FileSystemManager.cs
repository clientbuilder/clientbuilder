using System;
using System.IO;

namespace ClientBuilder.Core.Modules;

/// <inheritdoc />
public class FileSystemManager : IFileSystemManager
{
    /// <inheritdoc/>
    public virtual void CreateFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentNullException(nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    /// <inheritdoc/>
    public virtual void CreateFile(string filePath, string fileContent)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        File.WriteAllText(filePath, fileContent);
    }

    /// <inheritdoc/>
    public virtual bool IsFolderExists(string folderPath) =>
        Directory.Exists(folderPath);

    /// <inheritdoc/>
    public virtual bool IsFileExists(string filePath) =>
        File.Exists(filePath);
}