using System.Collections.Generic;
using System.Reflection;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Service that identify and extract needed information ot predefined assemblies.
/// </summary>
public interface IAssemblyScanner
{
    /// <summary>
    /// Fetch assembly types from loaded assemblies and registered rules.
    /// </summary>
    /// <returns></returns>
    IEnumerable<SourceAssemblyType> FetchSourceTypes();
}