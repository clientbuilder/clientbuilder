using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Exceptions;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// Abstract implementation of a runtime generation module that provides all abstractions and methods required by the Client Builder.
/// </summary>
public abstract class ScaffoldModule : IScaffoldModule
{
    private readonly IFileSystemManager fileSystemManager;
    private readonly IList<ScaffoldModuleFile> files;
    private readonly IList<ScaffoldModuleFolder> folders;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModule"/> class.
    /// </summary>
    /// <param name="fileSystemManager"></param>
    protected ScaffoldModule(IFileSystemManager fileSystemManager)
    {
        this.fileSystemManager = fileSystemManager;

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
    /// Type name of the module. The main use of this property is to give the name of the grouped modules.
    /// </summary>
    public string ScaffoldTypeName { get; protected set; }

    /// <summary>
    /// Instance type of the module - target application type of the generation (web, mobile, etc).
    /// </summary>
    public InstanceType Type { get; set; }

    /// <summary>
    /// Identification of the module by client type that allows easy modules grouping.
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
    /// Set source directory.
    /// </summary>
    /// <param name="sourceDirectory"></param>
    public void SetSourceDirectory(string sourceDirectory)
    {
        this.SourceDirectory = sourceDirectory;
    }

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
    /// Sync defined folders and files with the generated ones.
    /// </summary>
    public void Sync()
    {
        bool filesChecked = false;
        if (this.files != null && this.files.Count > 0)
        {
            filesChecked = true;
            this.Generated = true;
            foreach (var file in this.files)
            {
                var currentFilePath = Path.Combine(this.SourceDirectory, file.RelativePath, file.Name);
                this.Generated = this.Generated && this.fileSystemManager.IsFileExists(currentFilePath);
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
                this.Generated = this.Generated && this.fileSystemManager.IsFolderExists(currentFolderPath);
            }
        }
    }

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
}