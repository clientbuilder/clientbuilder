using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Exceptions;
using ClientBuilder.Options;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// Abstract implementation of a runtime generation module that provides all abstractions and methods required by the Client Builder.
/// </summary>
public abstract class ScaffoldModule : IScaffoldModule
{
    private readonly IList<ScaffoldModuleFile> files;
    private readonly IList<ScaffoldModuleFolder> folders;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModule"/> class.
    /// </summary>
    protected ScaffoldModule()
    {
        this.files = new List<ScaffoldModuleFile>();
        this.folders = new List<ScaffoldModuleFolder>();
    }

    /// <summary>
    /// Identifier of the module.
    /// </summary>
    public string Id => this.Name?.Replace(" ", "-").ToLower();

    /// <summary>
    /// Order of the module.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Name of the module.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Name of the client group from which the module is.
    /// </summary>
    public string ClientName { get; private set; }

    /// <summary>
    /// Id of the client group from which the module is.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Flag that indicates that the module is generated already.
    /// </summary>
    public bool Generated { get; set; }

    /// <summary>
    /// Startup directory of the module generation.
    /// </summary>
    public string SourceDirectory { get; private set; }

    /// <summary>
    /// Setup method that defines the required folder and files.
    /// </summary>
    /// <returns></returns>
    public abstract Task SetupAsync();

    /// <summary>
    /// Add module file.
    /// </summary>
    /// <param name="file"></param>
    public void AddFile(ScaffoldModuleFile file)
    {
        this.files.Add(file);
    }

    /// <summary>
    /// Add module folder.
    /// </summary>
    /// <param name="folder"></param>
    public void AddFolder(ScaffoldModuleFolder folder)
    {
        this.folders.Add(folder);
    }

    /// <summary>
    /// Get module file by reference id.
    /// </summary>
    /// <param name="referenceId"></param>
    /// <returns></returns>
    public ScaffoldModuleFile GetFile(string referenceId) =>
        this.files.FirstOrDefault(x => x.ReferenceId == referenceId);

    /// <summary>
    /// Get module folder by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ScaffoldModuleFolder GetFolder(string name) =>
        this.folders.FirstOrDefault(x => x.Name == name);

    /// <summary>
    /// Returns registered module files.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<ScaffoldModuleFile> GetFiles() =>
        new ReadOnlyCollection<ScaffoldModuleFile>(this.files);

    /// <summary>
    /// Returns registered module folders.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<ScaffoldModuleFolder> GetFolders() =>
        new ReadOnlyCollection<ScaffoldModuleFolder>(this.folders);

    /// <summary>
    /// Validates module properties.
    /// </summary>
    public void ValidateModule()
    {
        var moduleTypeName = this.GetType().FullName;
        if (string.IsNullOrWhiteSpace(this.Name))
        {
            throw new ClientBuilderException($"You must specify 'Name' for the {moduleTypeName}");
        }

        if (string.IsNullOrWhiteSpace(this.ClientId))
        {
            throw new ClientBuilderException($"You must specify 'ClientId' for the {moduleTypeName}");
        }
    }

    /// <summary>
    /// Consolidates fields that requires interaction with the options.
    /// </summary>
    /// <param name="options"></param>
    internal void ConsolidateModule(ClientBuilderOptions options)
    {
        this.SourceDirectory = options.ContentRootPath;
        var client = options.GetClient(this.ClientId);
        this.ClientName = client.Name;
        foreach (var folder in this.folders)
        {
            folder.RelativePath = string.IsNullOrWhiteSpace(folder.RelativePath) ?
                client.Path : Path.Combine(client.Path, folder.RelativePath);
        }

        foreach (var file in this.files)
        {
            file.RelativePath = string.IsNullOrWhiteSpace(file.RelativePath) ?
                client.Path : Path.Combine(client.Path, file.RelativePath);
        }
    }

    /// <summary>
    /// Sync defined folders and files with the generated ones.
    /// </summary>
    /// <param name="fileSystemManager"></param>
    internal void Sync(IFileSystemManager fileSystemManager)
    {
        bool filesChecked = false;
        if (this.files != null && this.files.Count > 0)
        {
            filesChecked = true;
            this.Generated = true;
            foreach (var file in this.files)
            {
                var currentFilePath = Path.Combine(this.SourceDirectory, file.RelativePath, file.Name);
                this.Generated = this.Generated && fileSystemManager.IsFileExists(currentFilePath);
            }
        }

        if (this.folders != null && this.folders.Count > 0)
        {
            if (filesChecked && !this.Generated)
            {
                return;
            }

            this.Generated = true;
            foreach (var folder in this.folders)
            {
                var currentFolderPath = Path.Combine(this.SourceDirectory, folder.RelativePath, folder.Name);
                this.Generated = this.Generated && fileSystemManager.IsFolderExists(currentFolderPath);
            }
        }
    }
}