using ClientBuilder.Attributes;

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

public enum NegativeEnum
{
    ExampleNegative100 = -100,
    ExampleZero = 0,
    ExamplePositive100 = 100,
}