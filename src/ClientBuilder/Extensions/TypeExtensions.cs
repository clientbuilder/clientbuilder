using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ClientBuilder.Extensions;

/// <summary>
/// Extensions for <see cref="Type"/>.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Returns whether a type has specified custom attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <typeparam name="TAttribute">Attribute type.</typeparam>
    /// <returns></returns>
    internal static bool HasCustomAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
        => type.HasCustomAttribute(typeof(TAttribute));

    /// <summary>
    /// Returns whether a type has specified custom attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    internal static bool HasCustomAttribute(this Type type, Type attributeType) =>
        type.GetCustomAttributes().Any(x => x.GetType() == attributeType);

    /// <summary>
    /// Checks whether a type is a child of a base type.
    /// </summary>
    /// <param name="type"></param>
    /// <typeparam name="TBase">Target base type.</typeparam>
    /// <returns></returns>
    internal static bool HasBaseClass<TBase>(this Type type)
    {
        var baseType = typeof(TBase);
        while (type != null && type != typeof(object))
        {
            if (type.BaseType == baseType)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpMethod"/> from action from the controller based on action attributes.
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    internal static System.Net.Http.HttpMethod GetMethodHttpDecoration(this MethodInfo methodInfo)
    {
        System.Net.Http.HttpMethod resultMethod = System.Net.Http.HttpMethod.Get;

        if (methodInfo.HasCustomAttribute<HttpGetAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Get;
        }
        else if (methodInfo.HasCustomAttribute<HttpPostAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Post;
        }
        else if (methodInfo.HasCustomAttribute<HttpPutAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Put;
        }
        else if (methodInfo.HasCustomAttribute<HttpDeleteAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Delete;
        }
        else if (methodInfo.HasCustomAttribute<HttpOptionsAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Options;
        }
        else if (methodInfo.HasCustomAttribute<HttpHeadAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Head;
        }
        else if (methodInfo.HasCustomAttribute<HttpPatchAttribute>())
        {
            resultMethod = System.Net.Http.HttpMethod.Patch;
        }

        return resultMethod;
    }

    /// <summary>
    /// Gets route from method that is considered as controller's action.
    /// </summary>
    /// <param name="actionInfo"></param>
    /// <returns></returns>
    internal static string GetActionRoute(this MethodInfo actionInfo)
    {
        string route = actionInfo.GetCustomAttribute<RouteAttribute>()?.Template;
        if (route == null)
        {
            route = actionInfo.GetCustomAttribute<HttpMethodAttribute>()?.Template;
        }

        if (route == null)
        {
            route = actionInfo.Name;
        }

        return route;
    }

    /// <summary>
    /// Gets route from type that considered as controller.
    /// </summary>
    /// <param name="controllerType"></param>
    /// <returns></returns>
    internal static string GetControllerRoute(this Type controllerType)
    {
        string controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;
        if (string.IsNullOrEmpty(controllerRoute))
        {
            var controllerName = controllerType.Name;
            if (controllerName.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - 10);
            }

            controllerRoute = $"/{controllerName}/";
        }

        return controllerRoute;
    }
}