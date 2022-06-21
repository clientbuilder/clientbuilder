﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
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
    public void GetAllControllerActions_OnDefaultInvocation_ShouldReturnProperActions()
    {
        var repository = GetSubject();
        var actions = repository.GetAllControllerActions();
        actions
            .Should()
            .HaveCount(4);

        actions
            .Select(x => new 
            {
                x.ControllerName,
                x.ActionName,
                x.Route,
                x.Method
            })
            .Should()
            .BeEquivalentTo(new List<object>
            {
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.Data),
                    Route = "/api/main/data",
                    Method = HttpMethod.Get,
                },
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.AddData),
                    Route = "/api/main/data",
                    Method = HttpMethod.Post,
                },
                new
                {
                    ControllerName = nameof(IncludedController),
                    ActionName = nameof(IncludedController.Check),
                    Route = "/api/main/check",
                    Method = HttpMethod.Post,
                },
                new
                {
                    ControllerName = nameof(SecondIncludedController),
                    ActionName = nameof(SecondIncludedController.DataItem),
                    Route = "/api/secondary/data/{id}",
                    Method = HttpMethod.Get,
                },
            });
    }
    
    [Fact]
    public void GetAllControllerActions_OnInvocationWithFilter_ShouldReturnProperActions()
    {
        var repository = GetSubject();
        var actions = repository
            .GetAllControllerActions(null, x => x.Type.Name != "IncludedController");
        actions
            .Should()
            .HaveCount(1);

        actions
            .Select(x => new 
            {
                x.ControllerName,
                x.ActionName,
                x.Route,
                x.Method
            })
            .Should()
            .BeEquivalentTo(new List<object>
            {
                new
                {
                    ControllerName = nameof(SecondIncludedController),
                    ActionName = nameof(SecondIncludedController.DataItem),
                    Route = "/api/secondary/data/{id}",
                    Method = HttpMethod.Get,
                },
            });
    }

    [Fact]
    public void GetAllManualRegisteredClasses_OnStandardInvocation_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<AssemblyScanner>(new OptionsAccessorFake());
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
        var descriptions = repository.GetAllManualRegisteredClasses();

        descriptions
            .Should()
            .HaveCount(2);

        descriptions
            .Select(x => x.Name)
            .Should()
            .BeEquivalentTo(nameof(SampleModelWithAttribute), nameof(SampleClassWithAttribute));
    }
    
    [Fact]
    public void GetAllManualRegisteredClasses_OnInvocationWithError_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<AssemblyScanner>(new OptionsAccessorFake());
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Throws<InvalidOperationException>();
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.GetAllManualRegisteredClasses();

        descriptions
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public void GetAllManualRegisteredClasses_OnInvocationWithFilterExpression_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<AssemblyScanner>(new OptionsAccessorFake());
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
        var descriptions = repository.GetAllManualRegisteredClasses(x => x.Type.Name.Contains("Model"));

        descriptions
            .Should()
            .HaveCount(1);

        descriptions
            .Select(x => x.Name)
            .Should()
            .BeEquivalentTo(nameof(SampleModelWithAttribute));
    }
    
    [Fact]
    public void GetAllRegisteredEnums_OnDefaultInvocation_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<AssemblyScanner>(new OptionsAccessorFake());
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
        var descriptions = repository.GetAllRegisteredEnums();

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
    public void GetAllRegisteredEnums_OnInvocationWithFilterExpression_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<AssemblyScanner>(new OptionsAccessorFake());
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
        var descriptions = repository.GetAllRegisteredEnums(x => x.Type.Name.Contains("Mode"));

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
    public void GetAllRegisteredEnums_OnInvocationWithError_ShouldReturnCorrectDescriptions()
    {
        var assemblyScannerMock = new Mock<AssemblyScanner>(new OptionsAccessorFake());
        assemblyScannerMock
            .Setup(x => x.FetchSourceTypes())
            .Throws<InvalidOperationException>();
        
        var repository = this.GetSubject(assemblyScannerMock);
        var descriptions = repository.GetAllRegisteredEnums();

        descriptions
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public void GetAllControllerActionsClasses_OnDefaultInvocation_ShouldReturnProperClasses()
    {
        var repository = GetSubject();
        var classes = repository.GetAllControllerActionsClasses();
        classes
            .Should()
            .HaveCount(1);

        var resultClass = classes.First();
        resultClass.Name
            .Should()
            .Be("SomeModel");
        resultClass.Properties
            .Should()
            .HaveCount(2);

        resultClass.Properties
            .Select(x => new
            {
                x.Name,
                Type = x.Type.Name,
            })
            .Should()
            .BeEquivalentTo(new List<object>
            {
                new
                {
                    Name = "Id",
                    Type = "string"
                },
                new
                {
                    Name = "Order",
                    Type = "int"
                }
            });
    }
    
    private ISourceRepository GetSubject(Mock<AssemblyScanner> assemblyScannerMock = null)
    {
        var optionsAccessor = new OptionsAccessorFake();
        var assemblyScanner = assemblyScannerMock?.Object ?? new AssemblyScanner(optionsAccessor);
            
        return new SourceRepository(
            assemblyScanner,
            new DescriptionExtractor(optionsAccessor, Mock.Of<ILogger<DescriptionExtractor>>()),
            Mock.Of<ILogger<SourceRepository>>());
    }
}