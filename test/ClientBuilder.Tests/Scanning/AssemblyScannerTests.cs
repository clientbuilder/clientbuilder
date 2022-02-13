using System.Linq;
using ClientBuilder.Core.Scanning;
using ClientBuilder.Options;
using ClientBuilder.TestAssembly.Controllers;
using ClientBuilder.TestAssembly.RuleSet;
using ClientBuilder.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClientBuilder.Tests.Scanning;

public class AssemblyScannerTests
{
    [Fact]
    public void FetchSourceTypes_OnDefaultAssemblyWithOneRules_ShouldReturnsProperAssemblyTypes()
    {
        var optionsAccessor = new OptionsAccessorFake();
        optionsAccessor.Value.ScanningRules.Clear();
        var scanningRules = new DecoratedControllerScanningRules();
        optionsAccessor.Value.AddScanningRules(scanningRules);
        var assemblyScanner = GetSubject(optionsAccessor);

        var types = assemblyScanner.FetchSourceTypes();
        types.Should().HaveCount(1);
        types.FirstOrDefault().Type.Should().Be(typeof(DecoratedController));
        types.FirstOrDefault().UsedRules.Should().Be(scanningRules);
    }

    [Fact]
    public void FetchSourceTypes_OnDefaultAssemblyWithNoRules_ShouldReturnsNoAssemblyTypes()
    {
        var optionsAccessor = new OptionsAccessorFake();
        optionsAccessor.Value.ScanningRules.Clear();
        var assemblyScanner = GetSubject(optionsAccessor);

        var types = assemblyScanner.FetchSourceTypes();
        types.Should().HaveCount(0);
    }
    
    [Fact]
    public void FetchSourceTypes_OnNoAssembliesWithNoRules_ShouldReturnsNoAssemblyTypes()
    {
        var optionsAccessor = new OptionsAccessorFake();
        optionsAccessor.Value.ScanningRules.Clear();
        optionsAccessor.Value.Assemblies.Clear();
        var assemblyScanner = GetSubject(optionsAccessor);

        var types = assemblyScanner.FetchSourceTypes();
        types.Should().HaveCount(0);
    }

    private IAssemblyScanner GetSubject(IOptions<ClientBuilderOptions> optionsAccessor)
    {
        return new AssemblyScanner(optionsAccessor);
    }
}