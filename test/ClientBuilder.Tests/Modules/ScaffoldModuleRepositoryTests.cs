using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using ClientBuilder.Tests.Fakes;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Modules;

public class ScaffoldModuleRepositoryTests
{
    [Fact]
    public async Task GetModuleAsync_OnStandardInvocation_ShouldReturnsCorrectResult()
    {
        var repository = this.GetSubject();
        var module = await repository.GetModuleAsync("simple-test-module");
        Assert.NotNull(module);
        module
            .Name
            .Should()
            .Be("Simple Test Module");
    }
    
    [Fact]
    public async Task GetModuleAsync_OnWrongInvocation_ShouldReturnsCorrectResult()
    {
        var repository = this.GetSubject();
        var module = await repository.GetModuleAsync("simple-test-module-123");
        Assert.Null(module);
    }
    
    [Fact]
    public async Task GetModulesAsync_OnStandardInvocation_ShouldReturnsCorrectResult()
    {
        var repository = this.GetSubject();
        var modules = await repository.GetModulesAsync();
        modules
            .Should()
            .HaveCount(1);

        modules
            .First()
            .Name
            .Should()
            .Be("Simple Test Module");
    }
    
    [Fact]
    public async Task GetModulesByClientIdAsync_OnStandardInvocation_ShouldReturnsCorrectResult()
    {
        var repository = this.GetSubject();
        var modules = await repository.GetModulesByClientIdAsync("test.client");
        modules
            .Should()
            .HaveCount(1);

        modules
            .First()
            .Name
            .Should()
            .Be("Simple Test Module");
    }
    
    [Fact]
    public async Task GetModulesByClientIdAsync_OnWrongInvocation_ShouldReturnsCorrectResult()
    {
        var repository = this.GetSubject();
        var modules = await repository.GetModulesByClientIdAsync("prod.client");
        modules
            .Should()
            .HaveCount(0);
    }

    private IScaffoldModuleRepository GetSubject()
    {
        var factoryMock = new Mock<IScaffoldModuleFactory>();
        factoryMock
            .Setup(x => x.BuildScaffoldModulesAsync())
            .Returns(Task.FromResult<IEnumerable<ScaffoldModule>>(new List<ScaffoldModule>
            {
                new SimpleTestModule(),
            }));

        return new ScaffoldModuleRepository(factoryMock.Object);
    }
}