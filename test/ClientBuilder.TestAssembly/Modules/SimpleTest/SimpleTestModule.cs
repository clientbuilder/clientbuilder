using System.IO;
using System.Threading.Tasks;
using ClientBuilder.Core.Modules;

namespace ClientBuilder.TestAssembly.Modules.SimpleTest;

public class SimpleTestModule : ScaffoldModule
{
    public SimpleTestModule(IFileSystemManager fileSystemManager)
        : base(fileSystemManager)
    {
        this.Name = "Simple Test Module";
        this.ClientId = "test.client";
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
            Template = new JsonFileTemplate(),
            ContextData = new { Data = "SimpleData" }
        });
    }
}