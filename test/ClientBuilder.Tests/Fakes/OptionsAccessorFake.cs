using ClientBuilder.Options;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Tests.Fakes;

public class OptionsAccessorFake : IOptions<ClientBuilderOptions>
{
    public OptionsAccessorFake()
    {
        this.Value = new ClientBuilderOptions();
        this.Value.AddAssembly("ClientBuilder.TestAssembly");
    }
    
    public ClientBuilderOptions Value { get; set; }
}