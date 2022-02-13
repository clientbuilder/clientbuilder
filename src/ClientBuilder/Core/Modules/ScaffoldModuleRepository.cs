using System.Collections.Generic;
using System.Linq;
using ClientBuilder.Common;

namespace ClientBuilder.Core.Modules;

/// <inheritdoc />
public class ScaffoldModuleRepository : IScaffoldModuleRepository
{
    private readonly IEnumerable<ScaffoldModule> modules;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleRepository"/> class.
    /// </summary>
    /// <param name="scaffoldModuleFactory"></param>
    public ScaffoldModuleRepository(IScaffoldModuleFactory scaffoldModuleFactory)
    {
        this.modules = scaffoldModuleFactory.BuildScaffoldModules();
    }

    /// <inheritdoc/>
    public ScaffoldModule GetModule(string moduleId) =>
        this.GetModules().FirstOrDefault(x => x.Id == moduleId);

    /// <inheritdoc/>
    public IList<ScaffoldModule> GetModules() =>
        this.GetConsolidateModules().ToList();

    /// <inheritdoc/>
    public IList<ScaffoldModule> GetModulesByClientId(string clientId) =>
        this.GetModules().Where(x => x.ClientId == clientId).ToList();

    /// <inheritdoc/>
    public IList<ScaffoldModule> GetModulesByInstance(InstanceType type) =>
        this.GetModules().Where(x => x.Type == type).ToList();

    private IEnumerable<ScaffoldModule> GetConsolidateModules()
    {
        foreach (var module in this.modules)
        {
            module.ValidateModule();
            module.Sync();
        }

        return this.modules;
    }
}