using System.Threading.Tasks;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.TestWithError;

public class TestWithErrorModule : ScaffoldModule
{
    public TestWithErrorModule()
    {
        this.Name = "Test With Error Module";
        this.ClientId = "test.client";
        this.Order = 1;
    }

    public override async Task SetupAsync()
    {
        this.AddFile(new ScaffoldModuleFile
        {
            Name = "file1.json",
            RelativePath = "folder2",
            Template = new T4FileTemplate(null),
            ContextData = new { Data = "SimpleData" },
            ReferenceId = "file1"
        });

        await Task.CompletedTask;
    }
}