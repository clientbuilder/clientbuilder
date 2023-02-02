using System.IO;
using ClientBuilder.Options;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Tests.Fakes;

public class OptionsAccessorFake : IOptions<ClientBuilderOptions>
{
    public static readonly string ExpectedClientPath = Path.Combine("..", "GenerationResultFolder");
    
    public OptionsAccessorFake()
    {
        this.Value = new ClientBuilderOptions();
        this.Value.ContentRootPath = Directory.GetCurrentDirectory();
        this.Value.AddAssembly("ClientBuilder.TestAssembly");
        this.Value.AddClient("test.client", "Test", 1, "GenerationResultFolder");
    }
    
    public ClientBuilderOptions Value { get; set; }
}