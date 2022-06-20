using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Controllers;
using ClientBuilder.Core.Modules;
using ClientBuilder.Exceptions;
using ClientBuilder.Models;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Controllers;

public class ScaffoldApiControllerTests
{
    [Fact]
    public void InstantiateController_OnNewDevelopmentInstance_ShouldCreateCorrectInstance()
    {
        var hostEnvironmentMock = new Mock<IWebHostEnvironment>();
        hostEnvironmentMock
            .SetupGet(x => x.EnvironmentName)
            .Returns(Environments.Development);

        this.GetSubject(hostEnvironmentMock, null, null);
    }
    
    [Fact]
    public void InstantiateController_OnNewProductionInstance_ShouldThrows()
    {
        var hostEnvironmentMock = new Mock<IWebHostEnvironment>();
        hostEnvironmentMock
            .SetupGet(x => x.EnvironmentName)
            .Returns(Environments.Production);

        var exception = Assert.Throws<DevelopmentOnlyException>(() => this.GetSubject(hostEnvironmentMock, null, null));
        exception
            .Message
            .Should()
            .Be("Client Builder cannot be accessed outside of development environment");
    }

    [Fact]
    public void CheckAvailability_OnInvocation_ShouldReturnOk()
    {
        var controller = this.GetSubject();
        var actionResult = controller.CheckAvailability();
        actionResult
            .Should()
            .BeOfType<OkResult>();
    }
    
    [Fact]
    public async Task GetAllModules_OnInvocation_ShouldReturnCorrectResult()
    {
        var repositoryResult = new List<ScaffoldModule>
        {
            new SimpleTestModule(Mock.Of<IFileSystemManager>()),
        };

        var repositoryMock = new Mock<IScaffoldModuleRepository>();
        repositoryMock
            .Setup(x => x.GetModulesAsync())
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(repositoryResult));
        
        var controller = this.GetSubject(null, repositoryMock);
        var actionResult = await controller.GetAllModules();

        actionResult
            .Should()
            .BeOfType<OkObjectResult>();

        ((OkObjectResult) actionResult)
            .Value
            .Should()
            .BeEquivalentTo(repositoryResult);
    }

    [Fact]
    public async Task GenerateModule_OnInvocationWithError_ShouldReturnsBadRequest()
    {
        var request = new GenerationByIdRequest();
        
        var repositoryMock = new Mock<IScaffoldModuleRepository>();
        repositoryMock
            .Setup(x => x.GetModulesAsync())
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(new List<ScaffoldModule>()));

        var generatorMock = new Mock<IScaffoldModuleGenerator>();
        generatorMock
            .Setup(x => x.GenerateAsync(It.IsAny<IEnumerable<string>>()))
            .Throws(new InvalidOperationException("Invalid generation operation"));
        
        var controller = this.GetSubject(null, repositoryMock, generatorMock);

        var actionResult = await controller.GenerateModule(request);
        actionResult
            .Should()
            .BeOfType<BadRequestObjectResult>();

        ((BadRequestObjectResult)actionResult)
            .Value
            .Should()
            .Be("Invalid generation operation");
    }
    
    [Fact]
    public async Task GenerateModule_OnInvocation_ShouldTriggerProperGenerator()
    {
        var fakeModuleInstance = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        var request = new GenerationByIdRequest { ModuleId = fakeModuleInstance.Id };
        var repositoryMock = new Mock<IScaffoldModuleRepository>();
        repositoryMock
            .Setup(x => x.GetModuleAsync(request.ModuleId))
            .Returns(Task.FromResult<ScaffoldModule>(fakeModuleInstance));
        
        var generatorMock = new Mock<IScaffoldModuleGenerator>();
        var controller = this.GetSubject(null, repositoryMock, generatorMock);

        await controller.GenerateModule(request);

        repositoryMock
            .Verify(x => x.GetModuleAsync(request.ModuleId));
        
        generatorMock
            .Verify(x => x.GenerateAsync(It.Is<IEnumerable<string>>(y => y.First() == fakeModuleInstance.Id)));
    }
    
    [Fact]
    public async Task GenerateModule_OnInvocationWithoutId_ShouldTriggerProperGenerator()
    {
        var fakeModuleInstance = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        var request = new GenerationByIdRequest { };
        var repositoryMock = new Mock<IScaffoldModuleRepository>();
        repositoryMock
            .Setup(x => x.GetModulesAsync())
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(new List<ScaffoldModule> { fakeModuleInstance }));
        
        var generatorMock = new Mock<IScaffoldModuleGenerator>();
        var controller = this.GetSubject(null, repositoryMock, generatorMock);

        await controller.GenerateModule(request);

        repositoryMock
            .Verify(x => x.GetModulesAsync());
        
        generatorMock
            .Verify(x => x.GenerateAsync(It.Is<IEnumerable<string>>(y => y.First() == fakeModuleInstance.Id)));
    }
    
    [Fact]
    public async Task GenerateModulesByInstance_OnInvocation_ShouldTriggerProperGenerator()
    {
        var fakeModuleInstance = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        var request = new GenerationByInstanceTypeRequest { InstanceType = fakeModuleInstance.Type };
        var repositoryMock = new Mock<IScaffoldModuleRepository>();
        repositoryMock
            .Setup(x => x.GetModulesByInstanceAsync(request.InstanceType))
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(new List<ScaffoldModule>{ fakeModuleInstance }));
        
        var generatorMock = new Mock<IScaffoldModuleGenerator>();
        var controller = this.GetSubject(null, repositoryMock, generatorMock);

        await controller.GenerateModulesByInstance(request);

        repositoryMock
            .Verify(x => x.GetModulesByInstanceAsync(request.InstanceType));
        
        generatorMock
            .Verify(x => x.GenerateAsync(It.Is<IEnumerable<string>>(y => y.First() == fakeModuleInstance.Id)));
    }
    
    [Fact]
    public async Task GenerateModulesByClientId_OnInvocation_ShouldTriggerProperGenerator()
    {
        var fakeModuleInstance = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        var request = new GenerationByClientIdRequest() { ClientId = fakeModuleInstance.ClientId };
        var repositoryMock = new Mock<IScaffoldModuleRepository>();
        repositoryMock
            .Setup(x => x.GetModulesByClientIdAsync(request.ClientId))
            .Returns(Task.FromResult<IReadOnlyCollection<ScaffoldModule>>(new List<ScaffoldModule>{ fakeModuleInstance }));
        
        var generatorMock = new Mock<IScaffoldModuleGenerator>();
        var controller = this.GetSubject(null, repositoryMock, generatorMock);

        await controller.GenerateModulesByClientId(request);

        repositoryMock
            .Verify(x => x.GetModulesByClientIdAsync(request.ClientId));
        
        generatorMock
            .Verify(x => x.GenerateAsync(It.Is<IEnumerable<string>>(y => y.First() == fakeModuleInstance.Id)));
    }
    
    private ScaffoldApiController GetSubject(
        Mock<IWebHostEnvironment> webHostEnvironmentMock = null,
        Mock<IScaffoldModuleRepository> scaffoldModuleRepositoryMock = null,
        Mock<IScaffoldModuleGenerator> scaffoldModuleGeneratorMock = null)
    {
        var hostEnvironmentMock = new Mock<IWebHostEnvironment>();
        hostEnvironmentMock
            .SetupGet(x => x.EnvironmentName)
            .Returns(Environments.Development);
        
        return new ScaffoldApiController(
            webHostEnvironmentMock?.Object ?? hostEnvironmentMock.Object,
            scaffoldModuleRepositoryMock?.Object,
            scaffoldModuleGeneratorMock?.Object,
            Mock.Of<ILogger<ScaffoldApiController>>());
    }
}