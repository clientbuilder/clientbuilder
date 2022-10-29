using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Core.Modules;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Modules;

public class ScaffoldModuleFactoryTests
{
    [Fact]
    public async Task BuildScaffoldModulesAsync_OnProperSetup_ShouldReturnCorrectModules()
    {
        var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        webHostEnvironmentMock
            .Setup(x => x.ContentRootPath)
            .Returns(Directory.GetCurrentDirectory);

        var mainModule = new SimpleTestModule(Mock.Of<IFileSystemManager>());
        
        var factory = this.GetSubject(
            new List<ScaffoldModule>
            {
                mainModule,
            },
            webHostEnvironmentMock);

        var modules = await factory.BuildScaffoldModulesAsync();

        modules
            .Should()
            .HaveCount(1);

        modules
            .First()
            .Id
            .Should()
            .Be(mainModule.Id);
        
        modules
            .First()
            .Name
            .Should()
            .Be(mainModule.Name);

        modules
            .First()
            .Order
            .Should()
            .Be(mainModule.Order);

        modules
            .First()
            .Type
            .Should()
            .Be(mainModule.Type);
        
        modules
            .First()
            .ScaffoldTypeName
            .Should()
            .Be(mainModule.ScaffoldTypeName);
        
        modules
            .First()
            .SourceDirectory
            .Should()
            .Be(Directory.GetCurrentDirectory());
    }
    
    private IScaffoldModuleFactory GetSubject(IEnumerable<ScaffoldModule> modules, Mock<IWebHostEnvironment> webHostEnvironmentMock = null)
    {
        return new ScaffoldModuleFactory(webHostEnvironmentMock?.Object ?? Mock.Of<IWebHostEnvironment>(), modules);
    }
}