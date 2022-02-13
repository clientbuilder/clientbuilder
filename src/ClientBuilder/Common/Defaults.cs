using System;
using System.Collections.Generic;

namespace ClientBuilder.Common;

/// <summary>
/// Defaults used by the Client Builder.
/// </summary>
public static class Defaults
{
    /// <summary>
    /// Primitive types and their name that defines the default definition of Client Builder for
    /// primitive type.
    /// </summary>
    public static readonly Dictionary<Type, string> PrimitiveTypes = new ()
    {
        { typeof(bool), "bool" },
        { typeof(string), "string" },
        { typeof(char), "char" },
        { typeof(Guid), "Guid" },
        { typeof(DateTime), "DateTime" },
        { typeof(TimeSpan), "TimeSpan" },
        { typeof(byte), "byte" },
        { typeof(short), "short" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(sbyte), "sbyte" },
        { typeof(ushort), "ushort" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
    };

    /// <summary>
    /// Map between a type and its client format. Default implementation follows JavaScript types.
    /// For example: 'Guid' in the C# can be represented as 'string' in JavaScript.
    /// </summary>
    public static readonly Dictionary<string, string> ClientRelatedTypes = new ()
    {
        { "bool", "boolean" },
        { "Boolean", "boolean" },
        { "string", "string" },
        { "String", "string" },
        { "char", "string" },
        { "Guid", "string" },
        { "DateTime", "string" },
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
}