using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClientBuilder.Exceptions;
using ClientBuilder.Options;
using FluentAssertions;
using Xunit;

namespace ClientBuilder.Tests.Options;

public class ClientBuilderOptionsTests
{
    [Fact]
    public void AddClient_OnSet_ShouldApplyCorrectlyThePath()
    {
        var options = this.GetSubject();
        options.AddClient("vue.app", "VueApp", 0, "MainFolder", "SubFolder");

        var client = options.Clients.First(x => x.Id == "vue.app");
        var expectedPath = Path.Combine("MainFolder", "SubFolder");

        client
            .Path
            .Should()
            .Be(expectedPath);
    }
    
    [Fact]
    public void AddClient_OnSetWithTwoDirectoriesBack_ShouldApplyCorrectlyThePath()
    {
        var options = this.GetSubject();
        options.AddClient("vue.app", "VueApp", 2, "MainFolder", "SubFolder");


        var client = options.Clients.First(x => x.Id == "vue.app");
        var expectedPath = Path.Combine("..", "..", "MainFolder", "SubFolder");

        client
            .Path
            .Should()
            .Be(expectedPath);
    }
    
    [Fact]
    public void GetClient_OnGet_ShouldReturnCorrectlyThePath()
    {
        var options = this.GetSubject();
        
        var expectedPath = Path.Combine("MainFolder", "SubFolder");
        options.Clients.Add(new ClientOptions
        {
            Id = "vue.app",
            Name = "VueApp",
            Path = expectedPath,
        });

        var client = options.GetClient("vue.app");

        client
            .Path
            .Should()
            .Be(expectedPath);
    }
    
    [Fact]
    public void GetClient_OnUsingInvalidApplication_ShouldThrows()
    {
        var options = this.GetSubject();
        
        var expectedPath = Path.Combine("MainFolder", "SubFolder");
        options.Clients.Add(new ClientOptions
        {
            Id = "vue.app",
            Name = "VueApp",
            Path = "vue.app",
        });


        var exception = Assert.Throws<ClientBuilderException>(() => options.GetClient("nuxt.app"));

        exception
            .Message
            .Should()
            .Be("There is no defined client with ID: 'nuxt.app'");
    }

    private ClientBuilderOptions GetSubject()
    {
        return new ClientBuilderOptions();
    }
}