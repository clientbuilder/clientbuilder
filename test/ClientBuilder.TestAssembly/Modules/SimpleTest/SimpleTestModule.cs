using System.Threading.Tasks;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.SimpleTest;

public class SimpleTestModule : ScaffoldModule
{
    public SimpleTestModule()
    {
        this.Name = "Simple Test Module";
        this.ClientId = "test.client";
        this.Order = 1;
    }

    public override async Task SetupAsync()
    {
        this.AddFolder(new ScaffoldModuleFolder
        {
            Name = "folder1",
        });
        
        this.AddFile(new ScaffoldModuleFile
        {
            Name = "file1.json",
            RelativePath = "folder1",
            Template = new JsonFileTemplate(),
            ContextData = new { Data = "SimpleData" },
            ReferenceId = "file1"
        });

        await Task.CompletedTask;
    }
}