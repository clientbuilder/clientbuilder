using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ClientBuilder.Common;
using ClientBuilder.Core.Scanning;
using ClientBuilder.TestAssembly.Controllers;
using ClientBuilder.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Scanning;

public class ControllerDescriptionExtractorTests
{
    [Fact]
    public void FetchControllerActions_OnDefaultInvocation_ShouldReturnProperActions()
    {
        var extractor = GetSubject();
        var actions = extractor.FetchControllerActions(new [] { "Main" });
        actions
            .Should()
            .HaveCount(4);

        actions
            .Select(x => new 
            {
                x.ControllerName,
                x.ActionName,
                x.Route,
                x.Method,
                x.MethodName,
                x.Authorized,
                x.ComplexArgument,
                StronglyTypedClientArgumentListString = x.GetStronglyTypedClientArgumentListString("{1}: {0}", TypeMappers.JavaScriptMapper),
                ClientArgumentNameListString = x.GetClientArgumentNameListString(),
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
                    MethodName = HttpMethod.Get.Method,
                    Authorized = false,
                    ComplexArgument = default(ArgumentDescription),
                    StronglyTypedClientArgumentListString = string.Empty,
                    ClientArgumentNameListString = string.Empty,
                },
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.AddData),
                    Route = "/api/main/data",
                    Method = HttpMethod.Post,
                    MethodName = HttpMethod.Post.Method,
                    Authorized = true,
                    ComplexArgument = new
                    {
                        Name = "model",
                        Type = new
                        {
                            Name = "SomeModel"
                        }
                    },
                    StronglyTypedClientArgumentListString = "model: SomeModel",
                    ClientArgumentNameListString = "model",
                },
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.Check),
                    Route = "/api/main/check",
                    Method = HttpMethod.Post,
                    MethodName = HttpMethod.Post.Method,
                    Authorized = true,
                    ComplexArgument = default(ArgumentDescription),
                    StronglyTypedClientArgumentListString = string.Empty,
                    ClientArgumentNameListString = string.Empty,
                },
                new
                {
                    ControllerName = nameof(SecondIncludedController),
                    ActionName = nameof(SecondIncludedController.DataItem),
                    Route = "/api/secondary/data/{id}",
                    Method = HttpMethod.Get,
                    MethodName = HttpMethod.Get.Method,
                    Authorized = true,
                    ComplexArgument = default(ArgumentDescription),
                    StronglyTypedClientArgumentListString = "id: string",
                    ClientArgumentNameListString = "id",
                },
            });
    }
    
    [Fact]
    public void FetchControllerActions_OnInvocationAboutAGroup_ShouldReturnProperActions()
    {
        var extractor = GetSubject();
        var actions = extractor.FetchControllerActions(new [] { "Private" });
        actions
            .Should()
            .HaveCount(1);

        actions
            .Select(x => new 
            {
                x.ControllerName,
                x.ActionName,
                x.Route,
                x.Method,
                x.MethodName,
                x.Authorized,
                x.ComplexArgument,
                StronglyTypedClientArgumentListString = x.GetStronglyTypedClientArgumentListString("{1}: {0}", TypeMappers.JavaScriptMapper),
                ClientArgumentNameListString = x.GetClientArgumentNameListString(),
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
                    MethodName = HttpMethod.Get.Method,
                    Authorized = true,
                    ComplexArgument = default(ArgumentDescription),
                    StronglyTypedClientArgumentListString = "id: string",
                    ClientArgumentNameListString = "id",
                },
            });
    }
    
    [Fact]
    public void FetchControllerActions_OnInvocationWithFilter_ShouldReturnProperActions()
    {
        var extractor = GetSubject();
        var actions = extractor
            .FetchControllerActions(null, x => x.Type.Name == "SecondIncludedController");
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
    public void FetchControllerActions_OnInvocationWithError_ShouldReturnProperActions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Throws<InvalidOperationException>();

        var extractor = GetSubject(assemblyScannerMock);

        var actions = extractor
            .FetchControllerActions();
        actions
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public void FetchControllerActions_OnSpecifiedController_ShouldConsumeAnyMethod()
    {
        var extractor = this.GetSubject();
        var actions = extractor.FetchControllerActions(new [] { "AllMethods" });

        actions
            .Select(x => x.Method)
            .ToList()
            .Should()
            .BeEquivalentTo(
                new List<HttpMethod>
                {
                    HttpMethod.Get,
                    HttpMethod.Post,
                    HttpMethod.Put,
                    HttpMethod.Delete,
                    HttpMethod.Patch,
                    HttpMethod.Head,
                    HttpMethod.Options,
                }
            );
    }
    
    [Fact]
    public void FetchActionsClasses_OnDefaultInvocation_ShouldReturnProperClasses()
    {
        var extractor = GetSubject();
        var classes = extractor.FetchActionsClasses();
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
    
    private IControllerDescriptionExtractor GetSubject(Mock<IAssemblyScanner> assemblyScannerMock = null)
    {
        var optionsAccessor = new OptionsAccessorFake();
        var assemblyScanner = assemblyScannerMock?.Object ?? new AssemblyScanner(optionsAccessor);
            
        return new ControllerDescriptionExtractor(
            assemblyScanner,
            new DescriptionExtractor(optionsAccessor, Mock.Of<ILogger<DescriptionExtractor>>()),
            Mock.Of<ILogger<ControllerDescriptionExtractor>>());
    }
}