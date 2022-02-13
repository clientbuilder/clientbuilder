using System.Collections.Generic;

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
    IEnumerable<ScaffoldModule> BuildScaffoldModules();
}