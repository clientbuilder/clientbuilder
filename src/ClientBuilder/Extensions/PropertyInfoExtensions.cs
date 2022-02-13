using System.Linq;
using System.Reflection;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for <see cref="PropertyInfo"/>.
/// </summary>
public static class PropertyInfoExtensions
{
    /// <summary>
    /// Checks whether a property is static or not.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static bool IsPropertyStatic(this PropertyInfo propertyInfo) =>
        propertyInfo.GetAccessors().Any(x => x.IsStatic);
}