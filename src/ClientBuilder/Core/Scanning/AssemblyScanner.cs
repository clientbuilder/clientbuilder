using System.Collections.Generic;
using System.Linq;
using ClientBuilder.Options;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Core.Scanning;

/// <inheritdoc />
public class AssemblyScanner : IAssemblyScanner
{
    private readonly ClientBuilderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyScanner"/> class.
    /// </summary>
    /// <param name="optionsAccessor"></param>
    public AssemblyScanner(IOptions<ClientBuilderOptions> optionsAccessor)
    {
        this.options = optionsAccessor.Value;
    }

    /// <inheritdoc/>
    public IEnumerable<SourceAssemblyType> FetchSourceTypes()
    {
        var assemblyTypes = new List<SourceAssemblyType>();
        var targetAssemblies = this.options.Assemblies;
        var scanningRules = this.options.ScanningRules;
        foreach (var assembly in targetAssemblies)
        {
            foreach (var scanningRulesItem in scanningRules)
            {
                var types = scanningRulesItem.FetchTypes(assembly);
                assemblyTypes.AddRange(types.Select(x => new SourceAssemblyType
                {
                    Type = x,
                    UsedRules = scanningRulesItem,
                }));
            }
        }

        return assemblyTypes;
    }
}