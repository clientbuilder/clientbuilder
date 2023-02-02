using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientBuilder.Attributes;
using ClientBuilder.Extensions;

namespace ClientBuilder.RuleSet;

/// <summary>
/// Default Client Builder rules for extracting controller types decorated with <see cref="IncludeControllerAttribute"/>.
/// </summary>
public class DefaultControllersScanningRules : IScanningRules
{
    /// <inheritdoc/>
    public IEnumerable<Type> FetchTypes(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(x => x.HasCustomAttribute<IncludeControllerAttribute>())
            .ToList();
    }
}