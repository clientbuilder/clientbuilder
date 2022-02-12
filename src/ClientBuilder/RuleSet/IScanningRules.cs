using System;
using System.Collections.Generic;
using System.Reflection;

namespace ClientBuilder.RuleSet;

/// <summary>
/// A set contract used for define a specific rules that has to be taken by the assembly scanner.
/// </summary>
public interface IScanningRules
{
    /// <summary>
    /// Returns types based on the specified rule.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    IEnumerable<Type> FetchTypes(Assembly assembly);
}
