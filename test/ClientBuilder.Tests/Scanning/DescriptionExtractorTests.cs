using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientBuilder.Core.Scanning;
using ClientBuilder.Options;
using ClientBuilder.TestAssembly.Models;
using ClientBuilder.Tests.Fakes;
using ClientBuilder.Tests.Samples;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ClientBuilder.Tests.Scanning;

public class DescriptionExtractorTests
{
    [Fact]
    public void ExtractTypeDescription_OnSimpleModelWithPrimitives_ShouldReturnProperDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(SimpleModelWithPrimitives));

        modelDescription
            .Should()
            .BeEquivalentTo(new
            {
                Name = "SimpleModelWithPrimitives",
                FullName = "ClientBuilder.Tests.Samples.SimpleModelWithPrimitives",
                IsCollection = false,
                IsNullable = true,
                IsEnum = false,
                IsGenericType = false,
                GenericTypes = default(TypeDescription[]),
                IsComplex = true
            });

        modelDescription
            .Properties
            .Should()
            .HaveCount(4);

        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "Name" && x.Type.IsComplex == false && x.Type.Name == "string" && !x.ReadOnly && x.DefaultValue == "null");
        
        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "Age" && x.Type.IsComplex == false && x.Type.Name == "int" && !x.ReadOnly && x.DefaultValue == default(int).ToString());
        
        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "Active" && x.Type.IsComplex == false && x.Type.Name == "bool" && !x.ReadOnly && x.DefaultValue == default(bool).ToString());
        
        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "StartDate" && x.Type.IsComplex == false && x.Type.Name == "DateTime" && !x.ReadOnly && x.DefaultValue == default(DateTime).ToString());
    }

    [Fact]
    public void ExtractTypeDescription_OnSelfContainedModel_ShouldUseTheParentDescriptionsProperly()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(SelfContainedModel));

        modelDescription
            .Properties
            .Should()
            .HaveCount(2);

        modelDescription
            .IsCollection
            .Should()
            .Be(false);
        
        var childProperty = modelDescription.Properties.First(x => x.Name == "Child");
        var childrenProperty = modelDescription.Properties.First(x => x.Name == "Children");

        childProperty
            .Type
            .FullName
            .Should()
            .Be(modelDescription.FullName);
        
        childProperty
            .Type
            .IsCollection
            .Should()
            .Be(false);
        
        childrenProperty
            .Type
            .FullName
            .Should()
            .Be(modelDescription.FullName);
        
        childrenProperty
            .Type
            .IsCollection
            .Should()
            .Be(true);
    }

    [Fact]
    public void ExtractTypeDescription_OnComplexGenericBasedThreeModel_ShouldReturnProperDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(MyDog));

        var baseModelType = modelDescription.BaseType;
            
        Assert.NotNull(baseModelType);

        baseModelType
            .IsGenericType
            .Should()
            .Be(true);

        baseModelType
            .GenericTypes
            .Should()
            .HaveCount(2);

        baseModelType
            .GenericTypeDescription
            .IsGenericType
            .Should()
            .Be(true);

        baseModelType
            .GenericTypeDescription
            .GenericTypes
            .Should()
            .HaveCount(2);

        baseModelType
            .GenericTypeDescription
            .GenericTypes
            .Select(x => x.Name)
            .Should()
            .BeEquivalentTo("TPreference", "TDate");

        baseModelType
            .HasOwnGenericBasedClass
            .Should()
            .Be(true);
        
        modelDescription
            .Properties
            .First(x => x.Name == "Friends")
            .Type
            .HasOwnGenericBasedClass
            .Should()
            .Be(false);
    }

    [Fact]
    public void ExtractTypeDescription_OnNullType_ShouldReturnInvalidDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(null);

        modelDescription
            .IsValid
            .Should()
            .Be(false);
    }

    [Fact]
    public void ExtractTypeDescription_OnNonProperInput_ShouldThrows()
    {
        var descriptionExtractor = this.GetSubject();
        Assert.Throws<ArgumentException>(() => descriptionExtractor.ExtractTypeDescription(typeof(Dog), typeof(Dog), null));
    }
    
    [InlineData(typeof(Type))]
    [InlineData(typeof(Type[]))]
    [InlineData(typeof(IEnumerable<Type>))]
    [InlineData(typeof(PropertyInfo))]
    [InlineData(typeof(MethodInfo))]
    [InlineData(typeof(FieldInfo))]
    [InlineData(typeof(MemberInfo))]
    [InlineData(typeof(Assembly))]
    [Theory]
    public void ExtractTypeDescription_OnReflectionType_ShouldReturnInvalidDescription(Type type)
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(type);

        modelDescription
            .IsValid
            .Should()
            .Be(false);
        
        modelDescription
            .Name
            .Should()
            .Be(type.Name);
        
        modelDescription
            .FullName
            .Should()
            .Be(type.FullName);
    }

    [Fact]
    public void ExtractInnerClassDescriptions_OnStandardInput_ShouldReturnsNoDescriptions()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(SomeModel));
        var innerClassDescriptions = descriptionExtractor.ExtractInnerClassDescriptions(modelDescription);

        innerClassDescriptions
            .Should()
            .HaveCount(0);
    }
    
    [Fact]
    public void ExtractInnerClassDescriptions_OnComplexModel_ShouldExtractProperDescriptions()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(MyDog));
        var innerClassDescriptions = descriptionExtractor.ExtractInnerClassDescriptions(modelDescription);

        innerClassDescriptions
            .Should()
            .HaveCount(4);
    }
    
    [Fact]
    public void ExtractUniqueEnumsFromClasses_OnMixedInput_ShouldReturnCorrectDescriptions()
    {
        var descriptionExtractor = this.GetSubject();
        var enumModelDescription = descriptionExtractor.ExtractTypeDescription(typeof(ModelWithEnum));
        var inheritedEnumModel = descriptionExtractor.ExtractTypeDescription(typeof(ModelInheritByModelWithEnum));
        var nonEnumModelDescription = descriptionExtractor.ExtractTypeDescription(typeof(SampleModelWIthoutAttribute));

        var uniqueEnums = descriptionExtractor.ExtractUniqueEnumsFromClasses(new List<TypeDescription> { enumModelDescription, inheritedEnumModel, nonEnumModelDescription });
        uniqueEnums
            .Should()
            .HaveCount(1);

        uniqueEnums
            .First()
            .Name
            .Should()
            .Be("DayOfWeek");

        uniqueEnums
            .First()
            .SourceType
            .Should()
            .Be(typeof(DayOfWeek));
    }

    [Fact]
    public void ExtractTypeDescription_OnUsingModelWithEnum_ShouldAssignCorrectEnumsPropertiesToTheDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(ModelWithEnum));

        modelDescription
            .Properties
            .First()
            .Type
            .EnumValues
            .Should()
            .BeEquivalentTo(new Dictionary<string, int>
            {
                {DayOfWeek.Sunday.ToString(), (int)DayOfWeek.Sunday},
                {DayOfWeek.Monday.ToString(), (int)DayOfWeek.Monday},
                {DayOfWeek.Tuesday.ToString(), (int)DayOfWeek.Tuesday},
                {DayOfWeek.Wednesday.ToString(), (int)DayOfWeek.Wednesday},
                {DayOfWeek.Thursday.ToString(), (int)DayOfWeek.Thursday},
                {DayOfWeek.Friday.ToString(), (int)DayOfWeek.Friday},
                {DayOfWeek.Saturday.ToString(), (int)DayOfWeek.Saturday},
            });

        modelDescription
            .Properties
            .First()
            .Type
            .EnumValueItems
            .Should()
            .BeEquivalentTo(new List<EnumValueItem>
            {
                new()
                {
                    Name = "Sunday",
                    OriginalName = DayOfWeek.Sunday.ToString(),
                    Key = "DAY_OF_WEEK_SUNDAY",
                    Value = (int)DayOfWeek.Sunday
                },
                new()
                {
                    Name = "Monday",
                    OriginalName = DayOfWeek.Monday.ToString(),
                    Key = "DAY_OF_WEEK_MONDAY",
                    Value = (int)DayOfWeek.Monday
                },
                new()
                {
                    Name = "Tuesday",
                    OriginalName = DayOfWeek.Tuesday.ToString(),
                    Key = "DAY_OF_WEEK_TUESDAY",
                    Value = (int)DayOfWeek.Tuesday
                },
                new()
                {
                    Name = "Wednesday",
                    OriginalName = DayOfWeek.Wednesday.ToString(),
                    Key = "DAY_OF_WEEK_WEDNESDAY",
                    Value = (int)DayOfWeek.Wednesday
                },
                new()
                {
                    Name = "Thursday",
                    OriginalName = DayOfWeek.Thursday.ToString(),
                    Key = "DAY_OF_WEEK_THURSDAY",
                    Value = (int)DayOfWeek.Thursday
                },
                new()
                {
                    Name = "Friday",
                    OriginalName = DayOfWeek.Friday.ToString(),
                    Key = "DAY_OF_WEEK_FRIDAY",
                    Value = (int)DayOfWeek.Friday
                },
                new()
                {
                    Name = "Saturday",
                    OriginalName = DayOfWeek.Saturday.ToString(),
                    Key = "DAY_OF_WEEK_SATURDAY",
                    Value = (int)DayOfWeek.Saturday
                },
            });
    }

    [Fact]
    public void ExtractTypeDescription_OnExtractingTypeOfEnumWithNegativeValues_ShouldReturnCorrectDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(NegativeEnum));

        modelDescription
            .EnumValues
            .Should()
            .BeEquivalentTo(new Dictionary<string, int>
            {
                {NegativeEnum.ExampleNegative100.ToString(), (int)NegativeEnum.ExampleNegative100},
                {NegativeEnum.ExampleZero.ToString(), (int)NegativeEnum.ExampleZero},
                {NegativeEnum.ExamplePositive100.ToString(), (int)NegativeEnum.ExamplePositive100},
            });

        modelDescription
            .EnumValueItems
            .OrderBy(x => x.Value)
            .Should()
            .BeEquivalentTo(new List<EnumValueItem>
            {
                new()
                {
                    Name = "Example Negative100",
                    OriginalName = NegativeEnum.ExampleNegative100.ToString(),
                    Key = "NEGATIVE_ENUM_EXAMPLE_NEGATIVE100",
                    Value = (int)NegativeEnum.ExampleNegative100
                },
                new()
                {
                    Name = "Example Zero",
                    OriginalName = NegativeEnum.ExampleZero.ToString(),
                    Key = "NEGATIVE_ENUM_EXAMPLE_ZERO",
                    Value = (int)NegativeEnum.ExampleZero
                },
                new()
                {
                    Name = "Example Positive100",
                    OriginalName = NegativeEnum.ExamplePositive100.ToString(),
                    Key = "NEGATIVE_ENUM_EXAMPLE_POSITIVE100",
                    Value = (int)NegativeEnum.ExamplePositive100
                },
            });
    }
    
    [Fact]
    public void ExtractTypeDescription_OnExtractingTypeOfEnumWithAttributes_ShouldReturnCorrectDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(ExampleEnum));

        modelDescription
            .EnumValues
            .Should()
            .BeEquivalentTo(new Dictionary<string, int>
            {
                {ExampleEnum.Example1.ToString(), (int)ExampleEnum.Example1},
                {ExampleEnum.Example2.ToString(), (int)ExampleEnum.Example2},
            });

        modelDescription
            .EnumValueItems
            .Should()
            .BeEquivalentTo(new List<EnumValueItem>
            {
                new()
                {
                    Name = "Example Enumeration 1",
                    OriginalName = ExampleEnum.Example1.ToString(),
                    Key = "EXAMPLE_ENUM_1",
                    Value = (int)ExampleEnum.Example1
                },
                new()
                {
                    Name = "Example Enumeration 2",
                    OriginalName = ExampleEnum.Example2.ToString(),
                    Key = "EXAMPLE_ENUM_2",
                    Value = (int)ExampleEnum.Example2
                },
            });
    }
    
    [Fact]
    public void ExtractTypeDescription_OnExtractingTypeOfNullableEnumWithAttributes_ShouldReturnCorrectDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var modelDescription = descriptionExtractor.ExtractTypeDescription(typeof(ExampleEnum?));

        modelDescription
            .EnumValues
            .Should()
            .BeEquivalentTo(new Dictionary<string, int>
            {
                {ExampleEnum.Example1.ToString(), (int)ExampleEnum.Example1},
                {ExampleEnum.Example2.ToString(), (int)ExampleEnum.Example2},
            });

        modelDescription
            .IsNullable
            .Should()
            .Be(true);
        
        modelDescription
            .EnumValueItems
            .Should()
            .BeEquivalentTo(new List<EnumValueItem>
            {
                new()
                {
                    Name = "Example Enumeration 1",
                    OriginalName = ExampleEnum.Example1.ToString(),
                    Key = "EXAMPLE_ENUM_1",
                    Value = (int)ExampleEnum.Example1
                },
                new()
                {
                    Name = "Example Enumeration 2",
                    OriginalName = ExampleEnum.Example2.ToString(),
                    Key = "EXAMPLE_ENUM_2",
                    Value = (int)ExampleEnum.Example2
                },
            });
    }
    
    [Fact]
    public void ExtractArgumentDescription_OnHardcodeArgumentType_ShouldHardcodeAsComplexTheDescription()
    {
        var descriptionExtractor = this.GetSubject();
        var argumentDescription = descriptionExtractor.ExtractArgumentDescription("body", typeof(object), true);

        argumentDescription
            .Name
            .Should()
            .Be("body");

        argumentDescription
            .Type
            .Name
            .Should()
            .Be("object");

        argumentDescription
            .Type
            .IsComplex
            .Should()
            .Be(true);

        argumentDescription
            .Type
            .IsClass
            .Should()
            .Be(true);
        
        argumentDescription
            .Type
            .IsInterface
            .Should()
            .Be(false);
        
        argumentDescription
            .Type
            .Hardcoded
            .Should()
            .Be(true);
    }
    
    private IDescriptionExtractor GetSubject(IOptions<ClientBuilderOptions> optionsAccessor = null)
    {
        var optionsAccessorInstance = optionsAccessor ?? new OptionsAccessorFake();
        return new DescriptionExtractor(optionsAccessorInstance, Mock.Of<ILogger<DescriptionExtractor>>());
    }
}