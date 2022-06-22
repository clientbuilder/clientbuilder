using System.IO;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.EmptyTest;

public class EmptyTestModule : ScaffoldModule
{
    public EmptyTestModule(IFileSystemManager fileSystemManager)
        : base(fileSystemManager)
    {
        this.Name = "Empty Test Module";
        this.ClientId = "test.client";
        this.Order = 1;
        this.IconUrl = "test.png";
        this.ScaffoldTypeName = "Test";
        this.Type = InstanceType.Desktop;
    }

    public override async Task SetupAsync()
    {
        await Task.CompletedTask;
    }
}