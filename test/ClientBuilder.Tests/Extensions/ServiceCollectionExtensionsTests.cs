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
    }
}