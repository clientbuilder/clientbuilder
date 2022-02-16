﻿namespace ClientBuilder.Core.Modules;

/// <summary>
/// Service responsible for work with files and folders from the system.
/// </summary>
public interface IFileSystemManager
{
    /// <summary>
    /// Creates a folder by specified path. If folder exists no folder will be created.
    /// </summary>
    /// <param name="folderPath"></param>
    void CreateFolder(string folderPath);

    /// <summary>
    /// Creates a file by specified path and content. If file exists no file will be created.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileContent"></param>
    void CreateFile(string filePath, string fileContent);

    /// <summary>
    /// Combine paths into a single path.
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    string CombinePaths(params string[] paths);

    /// <summary>
    /// Checks whether a folder exists.
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    bool IsFolderExists(string folderPath);

    /// <summary>
    /// Checks whether a file exists.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    bool IsFileExists(string filePath);
}