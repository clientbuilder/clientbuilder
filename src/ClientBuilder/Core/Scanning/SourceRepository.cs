using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientBuilder.DataAnnotations;
using ClientBuilder.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace ClientBuilder.Core.Scanning;

/// <inheritdoc />
public class SourceRepository : ISourceRepository
{
    private readonly IAssemblyScanner assemblyScanner;
    private readonly IDescriptionExtractor descriptionExtractor;
    private readonly ILogger<SourceRepository> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceRepository"/> class.
    /// </summary>
    /// <param name="assemblyScanner"></param>
    /// <param name="descriptionExtractor"></param>
    /// <param name="logger"></param>
    public SourceRepository(
        IAssemblyScanner assemblyScanner,
        IDescriptionExtractor descriptionExtractor,
        ILogger<SourceRepository> logger)
    {
        this.assemblyScanner = assemblyScanner;
        this.descriptionExtractor = descriptionExtractor;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public IEnumerable<ControllerAction> GetAllControllerActions()
    {
        try
        {
            var controllersTypes = this.assemblyScanner
                .FetchSourceTypes()
                .Where(x => x.Type.HasCustomAttribute<IncludeControllerAttribute>())
                .Select(x => x.Type);

            List<ControllerAction> resultActions = new List<ControllerAction>();
            foreach (var controllerType in controllersTypes)
            {
                var currentControllerActions = controllerType
                    .GetMethods()
                    .Where(x => x.HasCustomAttribute<IncludeActionAttribute>())
                    .ToList();

                foreach (var action in currentControllerActions)
                {
                    var currentAction = this.BuildAction(action, controllerType);

                    if (currentAction != null && resultActions.FirstOrDefault(x => x.Id == currentAction.Id) == null)
                    {
                        resultActions.Add(currentAction);
                    }
                }
            }

            return resultActions.OrderBy(x => x.ControllerName).ToList();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred during fetching controller actions");
            return new List<ControllerAction>();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> GetAllControllerActionsClasses()
    {
        try
        {
            var endpoints = this.GetAllControllerActions();
            var classes = new List<TypeDescription>();
            foreach (var endpoint in endpoints)
            {
                if (!endpoint.Response.Void && endpoint.Response.Type.IsComplex)
                {
                    classes.Add(endpoint.Response.Type);
                }

                classes.AddRange(endpoint.Arguments.Where(x => x.Type.IsComplex).Select(x => x.Type).ToList());
            }

            return this.descriptionExtractor.ExtractUniqueClassesFromClasses(classes);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred during fetching controller actions classes");
            return new List<TypeDescription>();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> GetAllRegisteredEnums()
    {
        try
        {
            var enumsTypeDescriptions = this.assemblyScanner
                .FetchSourceTypes()
                .Where(x => x.Type.IsEnum)
                .Select(x => this.descriptionExtractor.ExtractTypeDescription(x.Type))
                .ToList();

            return enumsTypeDescriptions;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred during fetching enums");
            return new List<TypeDescription>();
        }
    }

    private ControllerAction BuildAction(MethodInfo actionInfo, Type controllerType)
    {
        try
        {
            var actionAttribute = actionInfo.GetCustomAttribute<IncludeActionAttribute>();
            string controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;
            string actionRoute = this.GetActionRoute(actionInfo);

            var currentAction = new ControllerAction();
            currentAction.Id = $"{controllerType.FullName?.ToLower().Replace('.', '-')}-{actionInfo.Name.ToLower().Replace('.', '-')}";
            currentAction.ControllerName = controllerType.Name;
            currentAction.ActionName = actionInfo.Name;
            currentAction.Route = actionRoute.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                ? actionRoute
                : $"{controllerRoute}{actionRoute}";

            currentAction.Method = this.GetControllerActionHttpMethod(actionInfo);
            currentAction.Authorized = actionInfo.HasCustomAttribute<AuthorizeAttribute>() ||
                                       (controllerType.HasCustomAttribute<AuthorizeAttribute>() &&
                                        !actionInfo.HasCustomAttribute<AllowAnonymousAttribute>());
            currentAction.Response = this.descriptionExtractor.ExtractResponseDescription(actionAttribute.ResponseType);
            currentAction.Arguments = actionInfo
                .GetParameters()
                .Where(x => x.GetCustomAttribute<ExcludeElementAttribute>() == null)
                .Select(x => this.descriptionExtractor.ExtractArgumentDescription(x.Name, x.ParameterType))
                .ToList();

            return currentAction;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error on creating endpoint from controller ({FullName}) action ({Name})", actionInfo.Name, controllerType.FullName);
            return null;
        }
    }

    private string GetActionRoute(MethodInfo actionInfo)
    {
        string route = actionInfo.GetCustomAttribute<RouteAttribute>()?.Template;
        if (route == null)
        {
            route = actionInfo.GetCustomAttribute<HttpMethodAttribute>()?.Template;
        }

        return route;
    }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpMethod"/> from action from the controller based on action attributes.
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    private System.Net.Http.HttpMethod GetControllerActionHttpMethod(MethodInfo methodInfo)
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
}