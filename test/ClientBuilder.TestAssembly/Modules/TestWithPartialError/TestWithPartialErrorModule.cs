using System.IO;
using System.Threading.Tasks;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.TestWithPartialError;

public class TestWithPartialErrorModule : ScaffoldModule
{
    public TestWithPartialErrorModule(IFileSystemManager fileSystemManager)
        : base(fileSystemManager)
    {
        this.Name = "Test With Partial Error Module";
        this.ClientId = "test.error.partial";
        this.Order = 1;
        this.ScaffoldTypeName = "Test";
        this.Type = InstanceType.Undefined;
    }

    public override async Task SetupAsync()
    {
        this.AddFolder(new ScaffoldModuleFolder
        {
            Name = "folder1",
            RelativePath = Directory.GetCurrentDirectory(),
        });
        
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