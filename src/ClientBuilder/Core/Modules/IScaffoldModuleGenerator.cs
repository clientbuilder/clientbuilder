using System.Collections.Generic;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Models;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// Service that is responsible for the generation of the scaffold modules.
/// </summary>
public interface IScaffoldModuleGenerator
{
    /// <summary>
    /// Generates modules by using modules ids.
    /// </summary>
    /// <param name="modulesIds"></param>
    /// <returns></returns>
    Task<GenerationResult> GenerateAsync(IEnumerable<string> modulesIds);

    /// <summary>
    /// Generates modules from specified instance type.
    /// </summary>
    /// <param name="instanceType"></param>
    /// <returns></returns>
    Task<GenerationResult> GenerateAsync(InstanceType instanceType);

    /// <summary>
    /// Generate modules from specified client Id.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    Task<GenerationResult> GenerateAsync(string clientId);
}