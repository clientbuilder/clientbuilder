using System.Threading.Tasks;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.TestWithPartialError;

public class TestWithPartialErrorModule : ScaffoldModule
{
    public TestWithPartialErrorModule()
    {
        this.Name = "Test With Partial Error Module";
        this.ClientId = "test.client";
        this.Order = 1;
    }

    public override async Task SetupAsync()
    {
        this.AddFolder(new ScaffoldModuleFolder
        {
            Name = "folder-partial-error-module"
        });
        
        this.AddFile(new ScaffoldModuleFile
        {
            Name = "file1.json",
            RelativePath = "folder-partial-error-module",
            Template = new T4FileTemplate(null),
            ContextData = new { Data = "SimpleData" },
            ReferenceId = "file1"
        });

        await Task.CompletedTask;
    }
}