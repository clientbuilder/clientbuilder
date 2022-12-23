using System;
using System.Linq;
using System.Threading.Tasks;
using ClientBuilder.Core.Modules;
using ClientBuilder.Extensions;
using ClientBuilder.Options;
using Microsoft.Extensions.DependencyInjection;

namespace ClientBuilder.Host;

/// <summary>
/// Client Builder console application that starts an application in order invoke the generation action of the framework.
/// </summary>
public static class ClientBuilderConsoleApp
{
    /// <summary>
    /// Starts a Client Builder console application. Invoke that method in the Program.cs.
    /// Consider that is the minimal application setup and it is design to provide
    /// quick builder setup outside of your main application.
    /// Consider that the console application invokes directly the generation of all modules.
    /// </summary>
    /// <param name="optionsAction"></param>
    /// <param name="servicesAction"></param>
    /// <returns></returns>
    public static async Task StartAsync(
        Action<ClientBuilderOptions> optionsAction,
        Action<IServiceCollection> servicesAction = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        servicesAction?.Invoke(services);

        services.AddClientBuilder(optionsAction);

        var serviceProvider = services.BuildServiceProvider();

        var scaffoldModuleRepository = serviceProvider
            .GetRequiredService<IScaffoldModuleRepository>();

        var scaffoldModuleGenerator = serviceProvider
            .GetRequiredService<IScaffoldModuleGenerator>();

        var modulesToGenerate = await scaffoldModuleRepository.GetModulesAsync();
        var result = await scaffoldModuleGenerator.GenerateAsync(modulesToGenerate.Select(x => x.Id));
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"[CB_ERROR] {error}");
        }

        Console.WriteLine("Generation has been executed");
    }
}