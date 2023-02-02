using System;
using System.Collections.Generic;
using ClientBuilder.Attributes;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// A repository that provides all identified elements from the source application
/// which are exposed for the needs of the Client Builder.
/// </summary>
public interface ISourceRepository
{
    /// <summary>
    /// Get list of all registered enums decorated with <see cref="IncludeElementAttribute"/>.
    /// </summary>
    /// <param name="filter">Inject additional filter function into the enums fetching.</param>
    /// <returns></returns>
    IEnumerable<TypeDescription> FetchIncludedEnums(Func<SourceAssemblyType, bool> filter = null);

    /// <summary>
    /// Gets a list of all registered classes decorated by <see cref="IncludeElementAttribute"/>.
    /// </summary>
    /// <param name="filter">Inject additional filter function into the classes fetching.</param>
    /// <returns></returns>
    IEnumerable<TypeDescription> FetchIncludedClasses(Func<SourceAssemblyType, bool> filter = null);
}