namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Simplified description for a response extracted via reflection.
/// </summary>
public record ResponseDescription
{
    /// <summary>
    /// Indicates whether the response is void.
    /// </summary>
    public bool Void { get; set; }

    /// <summary>
    /// Type description of the response.
    /// </summary>
    public TypeDescription Type { get; set; }
}