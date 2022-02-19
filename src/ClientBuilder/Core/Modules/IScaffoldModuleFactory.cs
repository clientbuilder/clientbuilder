using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// Factory for creating scaffold modules.
/// </summary>
public interface IScaffoldModuleFactory
{
    /// <summary>
    /// Builds scaffold modules by using existing application setup and request.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<ScaffoldModule>> BuildScaffoldModulesAsync();
}