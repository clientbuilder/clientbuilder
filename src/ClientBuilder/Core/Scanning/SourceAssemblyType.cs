using System;
using ClientBuilder.RuleSet;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Type extracted from specific source.
/// </summary>
public class SourceAssemblyType
{
    /// <summary>
    /// Reflection type.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Contains the rules that have been used in order that type be found.
    /// </summary>
    public IScanningRules UsedRules { get; set; }
}