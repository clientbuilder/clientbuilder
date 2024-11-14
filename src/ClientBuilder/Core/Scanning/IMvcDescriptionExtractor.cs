using System;
using System.Collections.Generic;
using ClientBuilder.Attributes;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Service that extracts MVC specific descriptions.
/// </summary>
public interface IMvcDescriptionExtractor
{
    /// <summary>
    /// Get list of all controllers' actions based on the specified extraction options.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    IEnumerable<ControllerAction> FetchControllerActions(MvcExtractionOptions options = null);

    /// <summary>
    /// Get list of all used classes into the decorated actions (<see cref="IncludeActionAttribute"/>, <seealso cref="IncludeControllerAttribute"/>).
    /// </summary>
    /// <param name="controllerActions">If null then all controllers actions will be fetched.</param>
    /// <returns></returns>
    IEnumerable<TypeDescription> FetchActionsClasses(IEnumerable<ControllerAction> controllerActions = null);
}