using System;
using System.Collections.Generic;
using ClientBuilder.Attributes;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Service that extracts controller specific descriptions.
/// </summary>
public interface IControllerDescriptionExtractor
{
    /// <summary>
    /// Get list of all decorated (<see cref="IncludeActionAttribute"/>) actions from decorated controllers (<seealso cref="IncludeControllerAttribute"/>).
    /// </summary>
    /// <param name="groups">Collection of all include controller groups which must be used for the fetching. If collection is null or empty then it will ignore the group property. Consider that the groups are case sensitive.</param>
    /// <param name="filter">Inject additional filter function into the controllers actions fetching.</param>
    /// <returns></returns>
    IEnumerable<ControllerAction> FetchControllerActions(IEnumerable<string> groups = null, Func<SourceAssemblyType, bool> filter = null);

    /// <summary>
    /// Get list of all used classes into the decorated actions (<see cref="IncludeActionAttribute"/>, <seealso cref="IncludeControllerAttribute"/>).
    /// </summary>
    /// <param name="controllerActions">If null then all controllers actions will be fetched.</param>
    /// <returns></returns>
    IEnumerable<TypeDescription> FetchActionsClasses(IEnumerable<ControllerAction> controllerActions = null);
}