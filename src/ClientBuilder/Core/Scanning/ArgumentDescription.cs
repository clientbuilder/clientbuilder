﻿namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Simplified description for a method argument extracted via reflection.
/// </summary>
public record ArgumentDescription
{
    /// <summary>
    /// Name of the argument.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Type description of the argument.
    /// </summary>
    public TypeDescription Type { get; set; }
}