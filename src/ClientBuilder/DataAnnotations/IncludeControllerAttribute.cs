using System;

namespace ClientBuilder.DataAnnotations;

/// <summary>
/// Attribute that identifies decorated controller to be used as a target by the assembly scanner.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IncludeControllerAttribute : Attribute
{
}