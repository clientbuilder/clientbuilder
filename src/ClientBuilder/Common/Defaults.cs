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
        { typeof(object), "object" },
        { typeof(bool), "bool" },
        { typeof(string), "string" },
        { typeof(char), "char" },
        { typeof(Guid), "Guid" },
        { typeof(DateTime), "DateTime" },
        { typeof(DateTimeOffset), "DateTimeOffset" },
        { typeof(DateOnly), "DateOnly" },
        { typeof(TimeOnly), "TimeOnly" },
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
}