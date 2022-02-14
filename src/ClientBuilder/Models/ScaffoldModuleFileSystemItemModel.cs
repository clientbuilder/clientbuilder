using ClientBuilder.Core.Modules;

namespace ClientBuilder.Models;

/// <summary>
/// Model representing a file system item (file or folder) related to the <see cref="ScaffoldModule"/>.
/// </summary>
public class ScaffoldModuleFileSystemItemModel
{
    /// <summary>
    /// Name of the item.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Path of the item.
    /// </summary>
    public string Path { get; set; }
}