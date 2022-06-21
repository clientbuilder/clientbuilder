using ClientBuilder.DataAnnotations;

namespace ClientBuilder.Tests.Samples;

public enum ExampleEnum
{
    [EnumName("Example Enumeration 1")]
    [EnumKey("EXAMPLE_ENUM_1")]
    Example1 = 1,
    
    [EnumName("Example Enumeration 2")]
    [EnumKey("EXAMPLE_ENUM_2")]
    Example2 = 2,
}