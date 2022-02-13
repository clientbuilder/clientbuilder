using System;
using System.Linq;

namespace ClientBuilder.Common;

/// <summary>
/// Helper functions used by the Client Builder internally.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Transforms the first char of string in uppercase.
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static string ToFirstUpper(this string stringValue)
    {
        return string.Concat(stringValue.First().ToString().ToUpper(), stringValue.AsSpan(1));
    }

    /// <summary>
    /// Transforms the first char of string in lowercase.
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static string ToFirstLower(this string stringValue)
    {
        return string.Concat(stringValue.First().ToString().ToLower(), stringValue.AsSpan(1));
    }
}