using System;
using System.Collections.Generic;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Reflection-based service that extracts detailed type information from a specified type or element.
/// </summary>
public interface IDescriptionExtractor
{
    /// <summary>
    /// Extract description from type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parentType"></param>
    /// <param name="parentDescription"></param>
    /// <returns></returns>
    TypeDescription ExtractTypeDescription(
        Type type,
        Type parentType = null,
        TypeDescription parentDescription = null);

    /// <summary>
    /// Extract description from response type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    ResponseDescription ExtractResponseDescription(Type type);

    /// <summary>
    /// Extract description from argument type.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="hardcodeAsComplex"></param>
    /// <returns></returns>
    ArgumentDescription ExtractArgumentDescription(string name, Type type, bool hardcodeAsComplex);

    /// <summary>
    /// Extract descriptions from classes types.
    /// </summary>
    /// <param name="classes"></param>
    /// <returns></returns>
    IEnumerable<TypeDescription> ExtractUniqueClassesFromClasses(IEnumerable<TypeDescription> classes);

    /// <summary>
    /// Extract descriptions of inner classes from class type.
    /// </summary>
    /// <param name="classDescription"></param>
    /// <returns></returns>
    IEnumerable<TypeDescription> ExtractInnerClassDescriptions(TypeDescription classDescription);

    /// <summary>
    /// Extract enums description from classes types.
    /// </summary>
    /// <param name="classes"></param>
    /// <returns></returns>
    IEnumerable<TypeDescription> ExtractUniqueEnumsFromClasses(IEnumerable<TypeDescription> classes);
}