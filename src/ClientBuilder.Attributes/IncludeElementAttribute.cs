using System;

namespace ClientBuilder.Attributes;

/// <summary>
/// Attribute that identifies decorated model to be used as a target by the assembly scanner.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Struct)]
public class IncludeElementAttribute : Attribute
{
}