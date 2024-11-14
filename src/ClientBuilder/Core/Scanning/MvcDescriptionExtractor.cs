using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientBuilder.Attributes;
using ClientBuilder.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace ClientBuilder.Core.Scanning;

/// <inheritdoc />
public class MvcDescriptionExtractor : IMvcDescriptionExtractor
{
    private readonly IAssemblyScanner assemblyScanner;
    private readonly IDescriptionExtractor descriptionExtractor;
    private readonly ILogger<MvcDescriptionExtractor> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MvcDescriptionExtractor"/> class.
    /// </summary>
    /// <param name="assemblyScanner"></param>
    /// <param name="descriptionExtractor"></param>
    /// <param name="logger"></param>
    public MvcDescriptionExtractor(
        IAssemblyScanner assemblyScanner,
        IDescriptionExtractor descriptionExtractor,
        ILogger<MvcDescriptionExtractor> logger)
    {
        this.assemblyScanner = assemblyScanner;
        this.descriptionExtractor = descriptionExtractor;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public IEnumerable<ControllerAction> FetchControllerActions(MvcExtractionOptions options = null)
    {
        try
        {
            var extractionOptions = options ?? new MvcExtractionOptions();
            Func<SourceAssemblyType, bool> groupsFilter = _ => true;
            if (extractionOptions.Groups != null && extractionOptions.Groups.Any())
            {
                groupsFilter = x =>
                {
                    var attribute = x.Type.GetCustomAttribute<IncludeControllerAttribute>();
                    var attributeGroups = new List<string>();
                    if (attribute?.Groups != null && attribute.Groups.Any())
                    {
                        attributeGroups.AddRange(attribute.Groups);
                    }

                    return extractionOptions.Groups.Intersect(attributeGroups).Any();
                };
            }

            Func<SourceAssemblyType, bool> typeFilter = extractionOptions.Filter ?? (_ => true);
            var controllersTypes = this.assemblyScanner
                .FetchSourceTypes()
                .Where(x => x.Type.HasCustomAttribute<IncludeControllerAttribute>())
                .Where(groupsFilter)
                .Where(typeFilter)
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
    public IEnumerable<TypeDescription> FetchActionsClasses(IEnumerable<ControllerAction> controllerActions = null)
    {
        try
        {
            var actions = controllerActions ?? this.FetchControllerActions();
            var classes = new List<TypeDescription>();
            foreach (var action in actions)
            {
                if (!action.Response.Void && action.Response.Type.IsComplex)
                {
                    classes.Add(action.Response.Type);
                }

                classes.AddRange(action.Arguments.Where(x => x.Type.IsComplex).Select(x => x.Type).ToList());
            }

            return this.descriptionExtractor.ExtractUniqueClassesFromClasses(classes);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred during fetching controller actions classes");
            return new List<TypeDescription>();
        }
    }

    private ControllerAction BuildAction(MethodInfo actionInfo, Type controllerType)
    {
        try
        {
            var actionAttribute = actionInfo.GetCustomAttribute<IncludeActionAttribute>();

            var controllerRoute = controllerType.GetControllerRoute();
            string actionRoute = actionInfo.GetActionRoute();

            var currentAction = new ControllerAction();
            currentAction.Id = $"{controllerType.FullName?.ToLower().Replace('.', '-')}-{actionInfo.Name.ToLower().Replace('.', '-')}";
            currentAction.ControllerName = controllerType.Name;
            currentAction.ActionName = actionInfo.Name;
            currentAction.Route = this.BuildActionRoute(controllerRoute, actionRoute, actionInfo.Name);
            currentAction.Method = actionInfo.GetMethodHttpDecoration();
            currentAction.Authorized = actionInfo.HasCustomAttribute<AuthorizeAttribute>() ||
                                       (controllerType.HasCustomAttribute<AuthorizeAttribute>() &&
                                        !actionInfo.HasCustomAttribute<AllowAnonymousAttribute>());
            currentAction.Response = this.descriptionExtractor.ExtractResponseDescription(actionAttribute.ResponseType);
            currentAction.Arguments = actionInfo
                .GetParameters()
                .Where(x => x.GetCustomAttribute<ExcludeElementAttribute>() == null)
                .Select(x => this.descriptionExtractor.ExtractArgumentDescription(x.Name, x.ParameterType, x.GetCustomAttribute<HardcodeAsComplexAttribute>() != null))
                .ToList();

            return currentAction;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error on creating endpoint from controller ({FullName}) action ({Name})", controllerType.FullName, actionInfo.Name);
            return null;
        }
    }

    private string BuildActionRoute(string controllerRoute, string actionRoute, string actionName)
    {
        var route = string.Empty;
        if (actionRoute.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        {
            route = actionRoute;
        }
        else
        {
            if (controllerRoute.Contains("[action]"))
            {
                route = controllerRoute.Replace("[action]", actionName);
            }
            else
            {
                route = $"{controllerRoute}{actionRoute}";
            }
        }

        if (route.Contains("[action]"))
        {
            route = route.Replace("[action]", actionName);
        }

        return route;
    }
}