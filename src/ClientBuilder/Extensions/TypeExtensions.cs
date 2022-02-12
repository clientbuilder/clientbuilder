using System;
using System.Linq;
using System.Reflection;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Returns whether a type has specified custom attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <typeparam name="TAttribute">Attribute type.</typeparam>
    /// <returns></returns>
    public static bool HasCustomAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
        => type.HasCustomAttribute(typeof(TAttribute));

    /// <summary>
    /// Returns whether a type has specified custom attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    public static bool HasCustomAttribute(this Type type, Type attributeType) =>
        type.GetCustomAttributes().Any(x => x.GetType() == attributeType);
}