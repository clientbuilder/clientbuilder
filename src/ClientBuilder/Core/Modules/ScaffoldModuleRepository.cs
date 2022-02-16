using System.Collections.Generic;
using System.Linq;
using ClientBuilder.Common;

namespace ClientBuilder.Core.Modules;

/// <inheritdoc />
public class ScaffoldModuleRepository : IScaffoldModuleRepository
{
    private readonly IScaffoldModuleFactory scaffoldModuleFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldModuleRepository"/> class.
    /// </summary>
    /// <param name="scaffoldModuleFactory"></param>
    public ScaffoldModuleRepository(IScaffoldModuleFactory scaffoldModuleFactory)
    {
        this.scaffoldModuleFactory = scaffoldModuleFactory;
    }

    /// <inheritdoc/>
    public ScaffoldModule GetModule(string moduleId) =>
        this.GetModules().FirstOrDefault(x => x.Id == moduleId);

    /// <inheritdoc/>
    public IList<ScaffoldModule> GetModules() =>
        this.scaffoldModuleFactory.BuildScaffoldModules().ToList();

    /// <inheritdoc/>
    public IList<ScaffoldModule> GetModulesByClientId(string clientId) =>
        this.GetModules().Where(x => x.ClientId == clientId).ToList();

    /// <inheritdoc/>
    public IList<ScaffoldModule> GetModulesByInstance(InstanceType type) =>
        this.GetModules().Where(x => x.Type == type).ToList();
}