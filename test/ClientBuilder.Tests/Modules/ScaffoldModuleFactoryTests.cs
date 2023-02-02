using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Core.Modules;
using ClientBuilder.Options;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using ClientBuilder.Tests.Fakes;
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
        var mainModule = new SimpleTestModule();
        
        var factory = this.GetSubject(
            new List<ScaffoldModule>
            {
                mainModule,
            },
            new OptionsAccessorFake().Value);

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
            .ClientName
            .Should()
            .Be(mainModule.ClientName);
        
        modules
            .First()
            .SourceDirectory
            .Should()
            .Be(Directory.GetCurrentDirectory());
    }
    
    private IScaffoldModuleFactory GetSubject(IEnumerable<ScaffoldModule> modules, ClientBuilderOptions options = null)
    {
        return new ScaffoldModuleFactory(
            Mock.Of<IFileSystemManager>(),
            Microsoft.Extensions.Options.Options.Create(options ?? new ClientBuilderOptions()),
            modules);
    }
}