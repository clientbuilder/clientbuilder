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
    /// <returns></returns>
    IEnumerable<ControllerAction> GetAllControllerActions();

    /// <summary>
    /// Get list of all used classes into the decorated endpoints (<see cref="IncludeActionAttribute"/>, <seealso cref="IncludeControllerAttribute"/>).
    /// </summary>
    /// <returns></returns>
    IEnumerable<TypeDescription> GetAllControllerActionsClasses();

    /// <summary>
    /// Get list of all registered enums decorated <see cref="IncludeElementAttribute"/>.
    /// </summary>
    /// <returns></returns>
    IEnumerable<TypeDescription> GetAllRegisteredEnums();
}