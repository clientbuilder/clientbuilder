using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public virtual async Task<ScaffoldModule> GetModuleAsync(string moduleId) =>
        (await this.GetModulesAsync()).FirstOrDefault(x => x.Id == moduleId);

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<ScaffoldModule>> GetModulesAsync() =>
        (await this.scaffoldModuleFactory.BuildScaffoldModulesAsync())
        .OrderBy(x => x.Order)
        .ToList()
        .AsReadOnly();

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<ScaffoldModule>> GetModulesByClientIdAsync(string clientId) =>
        (await this.GetModulesAsync())
        .OrderBy(x => x.Order)
        .Where(x => x.ClientId == clientId)
        .ToList()
        .AsReadOnly();
}