using ClientBuilder.Common;
using FluentAssertions;
using Xunit;

namespace ClientBuilder.Tests.Common;

public class TypeMappersTests
{
    [InlineData("bool", "boolean")]
    [InlineData("Boolean", "boolean")]
    [InlineData("string", "string")]
    [InlineData("String", "string")]
    [InlineData("char", "string")]
    [InlineData("Guid", "string")]
    [InlineData("DateTime", "string")]
    [InlineData("DateOnly", "string")]
    [InlineData("TimeOnly", "string")]
    [InlineData("DateTimeOffset", "string")]
    [InlineData("TimeSpan", "string")]
    [InlineData("byte", "number")]
    [InlineData("short", "number")]
    [InlineData("int", "number")]
    [InlineData("long", "number")]
    [InlineData("sbyte", "number")]
    [InlineData("ushort", "number")]
    [InlineData("uint", "number")]
    [InlineData("ulong", "number")]
    [InlineData("Int32", "number")]
    [InlineData("Int64", "number")]
    [InlineData("float", "number")]
    [InlineData("double", "number")]
    [InlineData("decimal", "number")]
    [InlineData("SomeModel", "SomeModel")]
    [Theory]
    public void GetJavaScriptType_OnType_ShouldReturnCorrectJavaScriptType(string type, string expected)
    {
        TypeMappers
            .GetJavaScriptType(type)
            .Should()
            .Be(expected);
    }
}