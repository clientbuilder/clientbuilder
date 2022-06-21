using System.Linq;
using System.Reflection;
using ClientBuilder.Core.Modules;
using ClientBuilder.Extensions;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ClientBuilder.Tests.Extensions;

public class WebApplicationBuilderExtensionsTests
{
    [Fact]
    public void AddClientBuilder_OnInvocationInDevelopment_ShouldDoTheRegistration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;

        builder.AddClientBuilder(_ => { });
        builder
            .Services
            .Select(x => x.ServiceType?.Assembly)
            .Should()
            .Contain(typeof(ScaffoldModule).Assembly);
    }
    
    [Fact]
    public void AddClientBuilder_OnInvocationInProduction_ShouldIgnoreTheRegistration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Production;

        builder.AddClientBuilder(_ => { });
        builder
            .Services
            .Select(x => x.ServiceType?.Assembly)
            .Should()
            .NotContain(typeof(ScaffoldModule).Assembly);
    }
    
    [Fact]
    public void AddClientBuilder_OnModuleRegistration_ShouldRegistersTheModule()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;

        builder.AddClientBuilder(options =>
        {
            options.AddModule<SimpleTestModule>();
        });

        var serviceDescriptor = builder
            .Services
            .FirstOrDefault(x => x.ImplementationType == typeof(SimpleTestModule));

        Assert.NotNull(serviceDescriptor);
        
        serviceDescriptor
            .Lifetime
            .Should()
            .Be(ServiceLifetime.Scoped);
    }
}