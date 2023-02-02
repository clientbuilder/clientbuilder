using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ClientBuilder.Common;
using ClientBuilder.Core.Scanning;
using ClientBuilder.TestAssembly.Controllers;
using ClientBuilder.Tests.Fakes;
using ClientBuilder.Tests.Samples;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Scanning;

public class SourceRepositoryTests
{
    [Fact]
    public void FetchIncludedClasses_OnStandardInvocation_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Returns(new List<SourceAssemblyType>
            {
                new ()
                {
                    Type = typeof(SampleModelWithAttribute)
                },
                new ()
                {
                    Type = typeof(SampleModelWIthoutAttribute)
                },
                new ()
                {
                    Type = typeof(SampleClassWithAttribute)
                }
            });
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.FetchIncludedClasses();

        descriptions
            .Should()
            .HaveCount(2);

        descriptions
            .Select(x => x.Name)
            .Should()
            .BeEquivalentTo(nameof(SampleModelWithAttribute), nameof(SampleClassWithAttribute));
    }
    
    [Fact]
    public void FetchIncludedClasses_OnInvocationWithError_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Throws<InvalidOperationException>();
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.FetchIncludedClasses();

        descriptions
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public void FetchIncludedClasses_OnInvocationWithFilterExpression_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Returns(new List<SourceAssemblyType>
            {
                new ()
                {
                    Type = typeof(SampleModelWithAttribute)
                },
                new ()
                {
                    Type = typeof(SampleModelWIthoutAttribute)
                },
                new ()
                {
                    Type = typeof(SampleClassWithAttribute)
                }
            });
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.FetchIncludedClasses(x => x.Type.Name.Contains("Model"));

        descriptions
            .Should()
            .HaveCount(1);

        descriptions
            .Select(x => x.Name)
            .Should()
            .BeEquivalentTo(nameof(SampleModelWithAttribute));
    }
    
    [Fact]
    public void FetchIncludedEnums_OnDefaultInvocation_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Returns(new List<SourceAssemblyType>
            {
                new ()
                {
                    Type = typeof(DayOfWeek)
                },
                new ()
                {
                    Type = typeof(SampleClassWithAttribute)
                }
            });
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.FetchIncludedEnums();

        descriptions
            .Should()
            .HaveCount(1);

        descriptions
            .First()
            .Name
            .Should()
            .Be(nameof(DayOfWeek));
        
        descriptions
            .First()
            .SourceType
            .Should()
            .Be(typeof(DayOfWeek));
    }

    [Fact]
    public void FetchIncludedEnums_OnInvocationWithFilterExpression_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Returns(new List<SourceAssemblyType>
            {
                new ()
                {
                    Type = typeof(DayOfWeek)
                },
                new ()
                {
                    Type = typeof(ParallelExecutionMode)
                },
                new ()
                {
                    Type = typeof(SampleClassWithAttribute)
                }
            });
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.FetchIncludedEnums(x => x.Type.Name.Contains("Mode"));

        descriptions
            .Should()
            .HaveCount(1);

        descriptions
            .First()
            .Name
            .Should()
            .Be(nameof(ParallelExecutionMode));
        
        descriptions
            .First()
            .SourceType
            .Should()
            .Be(typeof(ParallelExecutionMode));
    }

    [Fact]
    public void FetchIncludedEnums_OnInvocationWithError_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<IAssemblyScanner>();
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Throws<InvalidOperationException>();
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.FetchIncludedEnums();

        descriptions
            .Should()
            .HaveCount(0);
    }

    private ISourceRepository GetSubject(Mock<IAssemblyScanner> assemblyScannerMock = null)
    {
        var optionsAccessor = new OptionsAccessorFake();
        var assemblyScanner = assemblyScannerMock?.Object ?? new AssemblyScanner(optionsAccessor);
            
        return new SourceRepository(
            assemblyScanner,
            new DescriptionExtractor(optionsAccessor, Mock.Of<ILogger<DescriptionExtractor>>()),
            Mock.Of<ILogger<SourceRepository>>());
    }
}