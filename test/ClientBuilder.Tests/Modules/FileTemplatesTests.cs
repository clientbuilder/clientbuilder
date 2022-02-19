using System.Collections.Generic;
using ClientBuilder.Core.Modules;
using ClientBuilder.Tests.Samples.Templates;
using ClientBuilder.Tests.Shared;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace ClientBuilder.Tests.Modules;

public class FileTemplatesTests
{
    [Fact]
    public void Render_OnT4FileTemplate_ShouldGenerateProperResult()
    {
        var fileTemplate = new T4FileTemplate(typeof(T4SampleFileTemplate));
        var fileContent = fileTemplate.Render(new Dictionary<string, object> { { "SampleData", "ClientBuilder123" } });
        fileContent
            .Should()
            .Be("ClientBuilder123");
    }

    [Fact]
    public void Render_OnHandlebarsFileTemplate_ShouldGenerateProperResult()
    {
        var fileTemplate = new HandlebarsFileTemplate("Samples", "Templates", "HandlebarsSampleFileTemplate.hbs");
        var fileContent = fileTemplate.Render(new { SampleData = "ClientBuilder123" });
        fileContent
            .Should()
            .Be("ClientBuilder123");
    }

    [Fact]
    public void Render_OnJsonFileTemplate_ShouldGenerateProperResult()
    {
        var fileTemplate = new JsonFileTemplate();
        var fileContent = fileTemplate.Render(new { SampleData = "ClientBuilder123" });
        fileContent = fileContent
            .Replace("\r\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace(" ", string.Empty);
        
        TestUtilities
            .NormalizeJson(fileContent)
            .Should()
            .Be(TestUtilities.NormalizeJson("{\"sampleData\":\"ClientBuilder123\"}"));
    }
    
    [Fact]
    public void Render_OnJsonFileTemplateWithCustomSerializationSettings_ShouldGenerateProperResult()
    {
        var fileTemplate = new JsonFileTemplate(new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
        });
        var fileContent = fileTemplate.Render(new { SampleData = "ClientBuilder123" });
        fileContent = fileContent
            .Replace("\r\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace(" ", string.Empty);
        
        TestUtilities
            .NormalizeJson(fileContent)
            .Should()
            .Be(TestUtilities.NormalizeJson("{\"sample_data\":\"ClientBuilder123\"}"));
    }
}