using System.Collections.Generic;
using System.IO;
using ClientBuilder.Options;
using FluentAssertions;
using Xunit;

namespace ClientBuilder.Tests.Options;

public class ClientBuilderOptionsTests
{
    [Fact]
    public void SetClientApplicationPath_OnSet_ShouldApplyCorrectlyThePath()
    {
        var options = this.GetSubject();
        options.SetClientApplicationPath("VueApp", 0, "MainFolder", "SubFolder");

        var appPath = options.ClientApplicationsPaths["VueApp"];
        var expectedPath = Path.Combine("MainFolder", "SubFolder");

        appPath
            .Should()
            .Be(expectedPath);
    }
    
    [Fact]
    public void SetClientApplicationPath_OnSetWithTwoDirectoriesBack_ShouldApplyCorrectlyThePath()
    {
        var options = this.GetSubject();
        options.SetClientApplicationPath("VueApp", 2, "MainFolder", "SubFolder");

        var appPath = options.ClientApplicationsPaths["VueApp"];
        var expectedPath = Path.Combine("..", "..", "MainFolder", "SubFolder");

        appPath
            .Should()
            .Be(expectedPath);
    }
    
    [Fact]
    public void GetClientApplicationPath_OnGet_ShouldReturnCorrectlyThePath()
    {
        var options = this.GetSubject();
        
        var expectedPath = Path.Combine("MainFolder", "SubFolder");
        options.ClientApplicationsPaths["VueApp"] = expectedPath;
        var appPath = options.GetClientApplicationPath("VueApp");

        appPath
            .Should()
            .Be(expectedPath);
    }
    
    [Fact]
    public void GetClientApplicationPath_OnUsingInvalidApplication_ShouldThrows()
    {
        var options = this.GetSubject();
        
        var expectedPath = Path.Combine("MainFolder", "SubFolder");
        options.ClientApplicationsPaths["VueApp"] = expectedPath;

        var exception = Assert.Throws<KeyNotFoundException>(() => options.GetClientApplicationPath("NuxtApp"));

        exception
            .Message
            .Should()
            .Be("There is no defined client application path for that identifier 'NuxtApp'");
    }

    private ClientBuilderOptions GetSubject()
    {
        return new ClientBuilderOptions();
    }
}