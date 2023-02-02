namespace ClientBuilder.Common;

/// <summary>
/// Type of generation result.
/// </summary>
public enum ScaffoldModuleGenerationStatusType
{
    /// <summary>
    /// Unsuccessful.
    /// </summary>
    Unsuccessful = 0,

    /// <summary>
    /// Successful.
    /// </summary>
    Successful = 1,

    /// <summary>
    /// Success with errors.
    /// </summary>
    SuccessfulWithErrors = 2,
}