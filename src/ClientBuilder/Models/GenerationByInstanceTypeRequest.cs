using ClientBuilder.Common;

namespace ClientBuilder.Models;

/// <summary>
/// Request model for creating a generation request by using specified instance type.
/// </summary>
public class GenerationByInstanceTypeRequest
{
    /// <summary>
    /// Target instance type.
    /// </summary>
    public InstanceType InstanceType { get; set; }
}