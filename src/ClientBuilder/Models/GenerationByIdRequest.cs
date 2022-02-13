namespace ClientBuilder.Models;

/// <summary>
/// Request model for creating a generation request by using specified module Id.
/// </summary>
public class GenerationByIdRequest
{
    /// <summary>
    /// Target module Id.
    /// </summary>
    public string ModuleId { get; set; }
}