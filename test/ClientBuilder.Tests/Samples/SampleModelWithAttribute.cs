using ClientBuilder.DataAnnotations;

namespace ClientBuilder.Tests.Samples;

[IncludeElement]
public class SampleModelWithAttribute
{
    public string Name { get; set; }
}