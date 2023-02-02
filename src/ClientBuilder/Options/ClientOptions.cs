namespace ClientBuilder.Options;

/// <summary>
/// Defines single client options used mainly to group modules.
/// </summary>
public class ClientOptions
{
    /// <summary>
    /// Identifier of the client.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Name of the client.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Relative path to the client folder.
    /// </summary>
    public string Path { get; init; }
}