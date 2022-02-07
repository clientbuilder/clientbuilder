namespace ClientBuilder;

/// <summary>
/// Type of the application instance.
/// </summary>
public enum InstanceType
{
    /// <summary>
    /// Undefined type of module.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Web module instance.
    /// </summary>
    Web = 1,

    /// <summary>
    /// Mobile module instance.
    /// </summary>
    Mobile = 2,

    /// <summary>
    /// Desktop module instance.
    /// </summary>
    Desktop = 3,
}