using System.Linq;
using System.Reflection;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for <see cref="MemberInfo"/>.
/// </summary>
public static class MemberInfoExtensions
{
    /// <summary>
    /// Check whether the <see cref="MemberInfo"/> has the specified custom attribute.
    /// </summary>
    /// <typeparam name="T">Attribute type.</typeparam>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static bool HasCustomAttribute<T>(this MemberInfo memberInfo) =>
        memberInfo.GetCustomAttributes(typeof(T), true).Any();
}