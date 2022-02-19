using System.Collections.Generic;
using ClientBuilder.Common;
using ClientBuilder.Core.Scanning;
using FluentAssertions;
using Xunit;

namespace ClientBuilder.Tests.Scanning;

public class TypeDescriptionTests
{
    [InlineData("bool", "boolean")]
    [InlineData("string", "string")]
    [InlineData("Guid", "string")]
    [InlineData("DateTime", "string")]
    [InlineData("TimeSpan", "string")]
    [InlineData("short", "number")]
    [InlineData("int", "number")]
    [InlineData("long", "number")]
    [InlineData("float", "number")]
    [InlineData("double", "number")]
    [InlineData("decimal", "number")]
    [InlineData("SimpleClassType", "SimpleClassType")]
    [Theory]
    public void GetClientType_OnJavaScriptMapper_ShouldReturnProperResult(string sourceType, string expectedClientType)
    {
        var description = new TypeDescription
        {
            Name = sourceType,
        };

        var clientType = description.GetClientType(TypeMappers.JavaScriptMapper);
        clientType.Should().Be(expectedClientType);
    }

    [Fact]
    public void GetClientArgumentNameListString_OnInvoke_ShouldReturnProperResult()
    {
        var description = new TypeDescription
        {
            Properties = new List<PropertyDescription>
            {
                new ()
                {
                    Name = "Id",
                    Type = new TypeDescription { Name = "string" },
                },
                new ()
                {
                    Name = "Age",
                    Type = new TypeDescription { Name = "int" },
                },
                new ()
                {
                    Name = "Active",
                    Type = new TypeDescription { Name = "bool" },
                },
            }
        };
        
        var argumentNameListString = description.GetClientArgumentNameListString();
        argumentNameListString.Should().Be("id, age, active");
    }
    
    [Fact]
    public void GetStronglyTypedClientArgumentListString_OnInvokeWithJavaScriptMapper_ShouldReturnProperResult()
    {
        var description = new TypeDescription
        {
            Properties = new List<PropertyDescription>
            {
                new ()
                {
                    Name = "Id",
                    Type = new TypeDescription { Name = "string" },
                },
                new ()
                {
                    Name = "Age",
                    Type = new TypeDescription { Name = "int" },
                },
                new ()
                {
                    Name = "Active",
                    Type = new TypeDescription { Name = "bool" },
                },
            }
        };
        
        var argumentNameListString = description.GetStronglyTypedClientArgumentListString("{1}: {0}", TypeMappers.JavaScriptMapper);
        argumentNameListString.Should().Be("id: string, age: number, active: boolean");
    }
}