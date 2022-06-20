using ClientBuilder.Common;
using ClientBuilder.Models;
using FluentAssertions;
using Xunit;

namespace ClientBuilder.Tests.Models;

public class RequestsTests
{
    [Fact]
    public void Get_OnCreatingGenerationByClientIdRequest_ShouldCreateCorrectInstance()
    {
        var request = new GenerationByClientIdRequest
        {
            ClientId = "vue"
        };

        request
            .ClientId
            .Should()
            .Be("vue");
    }
    
    [Fact]
    public void Get_OnCreatingGenerationByIdRequest_ShouldCreateCorrectInstance()
    {
        var request = new GenerationByIdRequest()
        {
            ModuleId = "vue.translations"
        };

        request
            .ModuleId
            .Should()
            .Be("vue.translations");
    }
    
    [Fact]
    public void Get_OnCreatingGenerationByInstanceTypeRequest_ShouldCreateCorrectInstance()
    {
        var request = new GenerationByInstanceTypeRequest()
        {
            InstanceType = InstanceType.Mobile
        };

        request
            .InstanceType
            .Should()
            .Be(InstanceType.Mobile);
    }
}