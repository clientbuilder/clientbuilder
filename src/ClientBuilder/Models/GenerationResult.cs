using System.Collections.Generic;
using ClientBuilder.Common;

namespace ClientBuilder.Models;

/// <summary>
/// Class that wraps the result of the generation execution.
/// </summary>
public class GenerationResult
{
    /// <summary>
    /// Collection of all errors from the generation.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Status of the generation.
    /// </summary>
    public ScaffoldModuleGenerationStatusType GenerationStatus { get; set; }
}