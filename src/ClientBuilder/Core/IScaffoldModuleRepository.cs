using System.Collections.Generic;
using ClientBuilder.Common;

namespace ClientBuilder.Core;

/// <summary>
/// Scaffold module repository for access defined Client Builder modules.
/// </summary>
public interface IScaffoldModuleRepository
{
    /// <summary>
    /// Gets a module by Id.
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    ScaffoldModule GetModule(string moduleId);

    /// <summary>
    /// Gets list of all scaffold modules.
    /// </summary>
    /// <returns></returns>
    IList<ScaffoldModule> GetModules();

    /// <summary>
    /// Get modules by client id.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    IList<ScaffoldModule> GetModulesByClientId(string clientId);

    /// <summary>
    /// Get modules by instance type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IList<ScaffoldModule> GetModulesByInstance(InstanceType type);
}