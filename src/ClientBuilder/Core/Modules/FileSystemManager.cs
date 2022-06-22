using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ClientBuilder.Core.Modules;

/// <inheritdoc />
public class FileSystemManager : IFileSystemManager
{
    private readonly ILogger<FileSystemManager> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemManager"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public FileSystemManager(ILogger<FileSystemManager> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public virtual bool CreateFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentNullException(nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            this.logger.LogDebug("Client Builder has created a folder ({FolderPath})", folderPath);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public virtual bool CreateFile(string filePath, string fileContent)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        File.WriteAllText(filePath, fileContent);
        this.logger.LogDebug("Client Builder has created a file ({FilePath})", filePath);
        return true;
    }

    /// <inheritdoc/>
    public virtual bool IsFolderExists(string folderPath)
    {
        this.logger.LogDebug("Check for a folder existence has been executed");
        return Directory.Exists(folderPath);
    }

    /// <inheritdoc/>
    public virtual bool IsFileExists(string filePath)
    {
        this.logger.LogDebug("Check for a file existence has been executed");
        return File.Exists(filePath);
    }
}