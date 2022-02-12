using System;
using System.Collections.Generic;

namespace ClientBuilder.DataAnnotations;

/// <summary>
/// Includes additional identification rule into the element. Multiple group input is possible.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class ElementGroupsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElementGroupsAttribute"/> class.
    /// </summary>
    /// <param name="groups"></param>
    public ElementGroupsAttribute(params string[] groups)
    {
        this.Groups = groups;
    }

    /// <summary>
    /// List of all groups for the current element.
    /// </summary>
    public IEnumerable<string> Groups { get; }
}