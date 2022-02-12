namespace ClientBuilder.Core;

/// <summary>
/// Folder implementation which will be created by the Client Builder.
/// </summary>
public class ScaffoldModuleFolder
{
    /// <summary>
    /// Name of the folder.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Relative path of the folder.
    /// </summary>
    public string RelativePath { get; set; }
}