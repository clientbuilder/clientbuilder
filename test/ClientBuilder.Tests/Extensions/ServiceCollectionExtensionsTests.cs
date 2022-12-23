using System.Linq;
using System.Reflection;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Extensions;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClientBuilder.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddClientBuilder_OnInvocationInDevelopment_ShouldDoTheRegistration()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddClientBuilder(_ => { });
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

        builder.Services.AddClientBuilder(opt =>
        {
            opt.IsDevelopment = false;
        });

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

        builder.Services.AddClientBuilder(options =>
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

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetService<IOptions<CorsOptions>>();
            
        Assert.NotNull(options);
        Assert.NotNull(options.Value);
            
        var expectedPolicy = options.Value.GetPolicy(Constants.ClientBuilderCorsPolicy);
            
        Assert.NotNull(expectedPolicy);
            
        expectedPolicy
            .AllowAnyMethod
            .Should()
            .Be(true);
            
        expectedPolicy
            .AllowAnyHeader
            .Should()
            .Be(true);
            
        expectedPolicy
            .AllowAnyOrigin
            .Should()
            .Be(false);
            
        expectedPolicy
            .SupportsCredentials
            .Should()
            .Be(false);
            
        expectedPolicy
            .Origins
            .Should()
            .BeEquivalentTo(Constants.ClientBuilderClientUrls);
    }
}