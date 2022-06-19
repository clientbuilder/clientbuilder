using System.Collections.Generic;

namespace ClientBuilder.Common;

/// <summary>
/// Static primitive type mapper that provides direct relation between C# type and some other client type.
/// </summary>
public static class TypeMappers
{
    /// <summary>
    /// JavaScript type mapper. Map is applicable for TypeScript.
    /// </summary>
    public static readonly IDictionary<string, string> JavaScriptMapper = new Dictionary<string, string>()
    {
        { "bool", "boolean" },
        { "Boolean", "boolean" },
        { "string", "string" },
        { "String", "string" },
        { "char", "string" },
        { "Guid", "string" },
        { "DateTime", "string" },
        { "DateOnly", "string" },
        { "TimeOnly", "string" },
        { "DateTimeOffset", "string" },
        { "TimeSpan", "string" },
        { "byte", "number" },
        { "short", "number" },
        { "int", "number" },
        { "long", "number" },
        { "sbyte", "number" },
        { "ushort", "number" },
        { "uint", "number" },
        { "ulong", "number" },
        { "Int32", "number" },
        { "Int64", "number" },
        { "float", "number" },
        { "double", "number" },
        { "decimal", "number" },
    };

    /// <summary>
    /// Returns JavaScript type based on default type mapper.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetJavaScriptType(string type)
    {
        if (!JavaScriptMapper.ContainsKey(type))
        {
            return type;
        }

        return JavaScriptMapper[type];
    }
}