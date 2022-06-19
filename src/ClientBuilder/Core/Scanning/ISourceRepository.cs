﻿using System;
using System.Collections;
using System.Collections.Generic;
using ClientBuilder.DataAnnotations;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// A repository that provides all identified elements from the source application
/// which are exposed for the needs of the Client Builder.
/// </summary>
public interface ISourceRepository
{
    /// <summary>
    /// Get list of all decorated (<see cref="IncludeActionAttribute"/>) actions from decorated controllers (<seealso cref="IncludeControllerAttribute"/>).
    /// </summary>
    /// <param name="filter">Inject additional filter function into the controllers actions fetching.</param>
    /// <returns></returns>
    IEnumerable<ControllerAction> GetAllControllerActions(Func<SourceAssemblyType, bool> filter = null);

    /// <summary>
    /// Get list of all used classes into the decorated endpoints (<see cref="IncludeActionAttribute"/>, <seealso cref="IncludeControllerAttribute"/>).
    /// </summary>
    /// <param name="controllerActions">If null then all controllers actions will be fetched.</param>
    /// <returns></returns>
    IEnumerable<TypeDescription> GetAllControllerActionsClasses(IEnumerable<ControllerAction> controllerActions = null);

    /// <summary>
    /// Get list of all registered enums decorated <see cref="IncludeElementAttribute"/>.
    /// </summary>
    /// <param name="filter">Inject additional filter function into the enums fetching.</param>
    /// <returns></returns>
    IEnumerable<TypeDescription> GetAllRegisteredEnums(Func<SourceAssemblyType, bool> filter = null);
}