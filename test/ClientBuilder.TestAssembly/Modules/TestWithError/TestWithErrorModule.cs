using System.IO;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.TestWithError;

public class TestWithErrorModule : ScaffoldModule
{
    public TestWithErrorModule(IFileSystemManager fileSystemManager)
        : base(fileSystemManager)
    {
        this.Name = "Test With Error Module";
        this.ClientId = "test.error";
        this.Order = 1;
        this.IconUrl = "test.png";
        this.ScaffoldTypeName = "Test";
        this.Type = InstanceType.Undefined;
    }

    public override async Task SetupAsync()
    {
        this.AddFile(new ScaffoldModuleFile
        {
            Name = "file1.json",
            RelativePath = Directory.GetCurrentDirectory(),
            Template = new T4FileTemplate(null),
            ContextData = new { Data = "SimpleData" },
            ReferenceId = "file1"
        });

        await Task.CompletedTask;
    }
}