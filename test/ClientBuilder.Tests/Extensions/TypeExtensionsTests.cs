using System.Net.Http;
using ClientBuilder.Extensions;
using ClientBuilder.TestAssembly.Controllers;
using ClientBuilder.TestAssembly.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ClientBuilder.Tests.Extensions;

public class TypeExtensionsTests
{
    [Fact]
    public void GetMethodHttpDecoration_WhenHttpGetAttributeIsPresent_ShouldReturnGet()
    {
        var methodInfo = typeof(SampleController).GetMethod(nameof(SampleController.GetAction));
        var result = methodInfo.GetMethodHttpDecoration();
        result.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public void GetMethodHttpDecoration_WhenHttpPostAttributeIsPresent_ShouldReturnPost()
    {
        var methodInfo = typeof(SampleController).GetMethod(nameof(SampleController.PostAction));
        var result = methodInfo.GetMethodHttpDecoration();
        result.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public void GetActionRoute_WhenRouteAttributeIsPresent_ShouldReturnRouteTemplate()
    {
        var methodInfo = typeof(SampleController).GetMethod(nameof(SampleController.GetAction));
        var result = methodInfo.GetActionRoute();
        result.Should().Be("sample/get");
    }

    [Fact]
    public void GetActionRoute_WhenNoRouteAttributeIsPresent_ShouldReturnMethodName()
    {
        var methodInfo = typeof(SampleController).GetMethod(nameof(SampleController.NoRouteAction));
        var result = methodInfo.GetActionRoute();
        result.Should().Be("NoRouteAction");
    }

    [Fact]
    public void GetControllerRoute_WhenRouteAttributeIsPresent_ShouldReturnRouteTemplate()
    {
        var result = typeof(IncludedController).GetControllerRoute();
        result.Should().Be("/api/main/");
    }

    [Fact]
    public void GetControllerRoute_WhenNoRouteAttributeIsPresent_ShouldReturnDefaultRoute()
    {
        var result = typeof(QuickController).GetControllerRoute();
        result.Should().Be("/Quick/");
    }

    [Fact]
    public void GetControllerRoute_WhenTemplateRoutePresent_ShouldReturnCorrectRoute()
    {
        var result = typeof(TemplateController).GetControllerRoute();
        result.Should().Be("/Template/");
    }
    
    [Fact]
    public void HasBaseClass_WhenPassingController_ShouldReturnCorrectResult()
    {
        typeof(QuickController).HasBaseClass<ControllerBase>().Should().BeTrue();
        typeof(IncludedController).HasBaseClass<ControllerBase>().Should().BeTrue();
        typeof(IncludedController).HasBaseClass<Controller>().Should().BeTrue();
        typeof(SomeModel).HasBaseClass<ControllerBase>().Should().BeFalse();
    }
}