using System;

namespace ClientBuilder.Attributes;

/// <summary>
/// Attribute that identifies decorated action to be used as a target by the assembly scanner.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class IncludeActionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeActionAttribute"/> class.
    /// </summary>
    /// <param name="responseType"></param>
    public IncludeActionAttribute(Type responseType)
    {
        this.ResponseType = responseType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeActionAttribute"/> class.
    /// </summary>
    public IncludeActionAttribute()
    {
    }

    /// <summary>
    /// Expected response data type from the action. If value is 'null' it means the action returns only status code.
    /// </summary>
    public Type ResponseType { get; private set; }
}