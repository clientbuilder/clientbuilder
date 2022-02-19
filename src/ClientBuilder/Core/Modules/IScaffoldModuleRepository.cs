using System.Collections.Generic;
using System.Threading.Tasks;
using ClientBuilder.Common;

namespace ClientBuilder.Core.Modules;

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
    Task<ScaffoldModule> GetModuleAsync(string moduleId);

    /// <summary>
    /// Gets list of all scaffold modules.
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyCollection<ScaffoldModule>> GetModulesAsync();

    /// <summary>
    /// Get modules by client id.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ScaffoldModule>> GetModulesByClientIdAsync(string clientId);

    /// <summary>
    /// Get modules by instance type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ScaffoldModule>> GetModulesByInstanceAsync(InstanceType type);
}