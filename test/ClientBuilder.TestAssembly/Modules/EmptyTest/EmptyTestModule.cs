using System.Threading.Tasks;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.EmptyTest;

public class EmptyTestModule : ScaffoldModule
{
    public EmptyTestModule()
    {
        this.Name = "Empty Test Module";
        this.ClientId = "test.client";
        this.Order = 1;
    }

    public override async Task SetupAsync()
    {
        await Task.CompletedTask;
    }
}