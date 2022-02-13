using ClientBuilder.Core.Scanning;
using ClientBuilder.Options;
using ClientBuilder.Tests.Fakes;
using ClientBuilder.Tests.Samples;
using FluentAssertions;
using Microsoft.Extensions.Options;
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
                FullNameWithGeneric = "ClientBuilder.Tests.Samples.SimpleModelWithPrimitives",
                ClientTypeName = "SimpleModelWithPrimitives",
                IsCollection = false,
                IsNullable = true,
                IsEnum = false,
                IsGenericType = false,
                GenericType = default(TypeDescription),
                IsComplex = true
            });

        modelDescription
            .Properties
            .Should()
            .HaveCount(4);

        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "Name" && x.Type.IsComplex == false && x.Type.Name == "string");
        
        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "Age" && x.Type.IsComplex == false && x.Type.Name == "int");
        
        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "Active" && x.Type.IsComplex == false && x.Type.Name == "bool");
        
        modelDescription
            .Properties
            .Should()
            .ContainSingle(x => x.Name == "StartDate" && x.Type.IsComplex == false && x.Type.Name == "DateTime");
    }
    
    private IDescriptionExtractor GetSubject(IOptions<ClientBuilderOptions> optionsAccessor = null)
    {
        var optionsAccessorInstance = optionsAccessor ?? new OptionsAccessorFake();
        return new DescriptionExtractor(optionsAccessorInstance);
    }
}