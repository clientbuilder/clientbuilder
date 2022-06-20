using System;

namespace ClientBuilder.DataAnnotations;

/// <summary>
/// Attribute that identifies decorated controller to be used as a target by the assembly scanner.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IncludeControllerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeControllerAttribute"/> class.
    /// </summary>
    /// <param name="groups"></param>
    public IncludeControllerAttribute(params string[] groups)
    {
        this.Groups = groups;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeControllerAttribute"/> class.
    /// </summary>
    public IncludeControllerAttribute()
    {
    }

    /// <summary>
    /// Property that allows to group the attribute.
    /// </summary>
    public string[] Groups { get; set; }
}