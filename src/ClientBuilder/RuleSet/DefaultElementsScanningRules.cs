using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientBuilder.DataAnnotations;
using ClientBuilder.Extensions;

namespace ClientBuilder.RuleSet;

/// <summary>
/// Default Client Builder rules for extracting model types decorated with <see cref="IncludeElementAttribute"/>.
/// </summary>
public class DefaultElementsScanningRules : IScanningRules
{
    /// <inheritdoc/>
    public IEnumerable<Type> FetchTypes(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(x => x.HasCustomAttribute<IncludeElementAttribute>())
            .ToList();
    }
}