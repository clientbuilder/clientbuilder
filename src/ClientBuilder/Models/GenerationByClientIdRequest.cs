namespace ClientBuilder.Models;

/// <summary>
/// Request model for creating a generation request by using specified client Id.
/// </summary>
public class GenerationByClientIdRequest
{
    /// <summary>
    /// Target client Id.
    /// </summary>
    public string ClientId { get; set; }
}