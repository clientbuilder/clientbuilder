using System;
using System.Collections.Generic;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Options container that contains the instruction which exact actions need to be extracted from the scanned assemblies.
/// </summary>
public class MvcExtractionOptions
{
    /// <summary>
    /// List of the groups of all target controllers that need to be used for the extraction.
    /// </summary>
    public IList<string> Groups { get; set; }

    /// <summary>
    /// Function that filters the types from where the controllers are extracted.
    /// </summary>
    public Func<SourceAssemblyType, bool> Filter { get; set; }
}