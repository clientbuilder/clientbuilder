using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using ClientBuilder.Core.Scanning;
using ClientBuilder.TestAssembly.Controllers;
using ClientBuilder.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Scanning;

public class SourceRepositoryTests
{
    [Fact]
    public void GetAllControllerActions_OnDefaultInvocation_ShouldReturnProperActions()
    {
        var repository = GetSubject();
        var actions = repository.GetAllControllerActions();
        actions
            .Should()
            .HaveCount(4);

        actions
            .Select(x => new 
            {
                x.ControllerName,
                x.ActionName,
                x.Route,
                x.Method
            })
            .Should()
            .BeEquivalentTo(new List<object>
            {
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.Data),
                    Route = "/api/main/data",
                    Method = HttpMethod.Get,
                },
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.AddData),
                    Route = "/api/main/data",
                    Method = HttpMethod.Post,
                },
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.Check),
                    Route = "/api/main/check",
                    Method = HttpMethod.Post,
                },
                new
                {
                    ControllerName = nameof(SecondIncludedController),
                    ActionName = nameof(SecondIncludedController.DataItem),
                    Route = "/api/secondary/data/{id}",
                    Method = HttpMethod.Get,
                },
            });
    }
    
    [Fact]
    public void GetAllControllerActions_OnInvocationWithFilter_ShouldReturnProperActions()
    {
        var repository = GetSubject();
        var actions = repository
            .GetAllControllerActions(x => x.Type.Name != "IncludedController");
        actions
            .Should()
            .HaveCount(1);

        actions
            .Select(x => new 
            {
                x.ControllerName,
                x.ActionName,
                x.Route,
                x.Method
            })
            .Should()
            .BeEquivalentTo(new List<object>
            {
                new
                {
                    ControllerName = nameof(SecondIncludedController),
                    ActionName = nameof(SecondIncludedController.DataItem),
                    Route = "/api/secondary/data/{id}",
                    Method = HttpMethod.Get,
                },
            });
    }

    [Fact]
    public void GetAllControllerActionsClasses_OnDefaultInvocation_ShouldReturnProperClasses()
    {
        var repository = GetSubject();
        var classes = repository.GetAllControllerActionsClasses();
        classes
            .Should()
            .HaveCount(1);

        var resultClass = classes.First();
        resultClass.Name
            .Should()
            .Be("SomeModel");
        resultClass.Properties
            .Should()
            .HaveCount(2);

        resultClass.Properties
            .Select(x => new
            {
                x.Name,
                Type = x.Type.Name,
            })
            .Should()
            .BeEquivalentTo(new List<object>
            {
                new
                {
                    Name = "Id",
                    Type = "string"
                },
                new
                {
                    Name = "Order",
                    Type = "int"
                }
            });
    }
    
    private ISourceRepository GetSubject()
    {
        var optionsAccessor = new OptionsAccessorFake();
        return new SourceRepository(
            new AssemblyScanner(optionsAccessor),
            new DescriptionExtractor(optionsAccessor),
            Mock.Of<ILogger<SourceRepository>>());
    }
}