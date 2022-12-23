﻿using System;
using ClientBuilder.Extensions;
using ClientBuilder.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientBuilder.Host;

/// <summary>
/// Client Builder web API that starts an application in order to host Client Builder API.
/// </summary>
public static class ClientBuilderWebApi
{
    /// <summary>
    /// Starts a Client Builder ASP.NET web API. Invoke that method in the Program.cs.
    /// Consider that is the minimal server setup and it is design to provide
    /// quick builder setup outside of your main application.
    /// Make sure you have reference your ASP.NET application by that server assembly.
    /// </summary>
    /// <param name="hostUrl"></param>
    /// <param name="optionsAction"></param>
    /// <param name="builderAction"></param>
    public static void Start(
        string hostUrl,
        Action<ClientBuilderOptions> optionsAction,
        Action<WebApplicationBuilder> builderAction = null)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development,
        });

        builderAction?.Invoke(builder);

        builder
            .WebHost
            .UseKestrel()
            .UseUrls(hostUrl);

        builder.Services.AddClientBuilder(optionsAction);

        builder.Services.AddControllers()
            .ConfigureApplicationPartManager(x =>
            {
                x.ApplicationParts.Clear();
                x.ApplicationParts.Add(new AssemblyPart(typeof(ClientBuilderWebApi).Assembly));
            });

        var app = builder.Build();

        app.UseRouting();

        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}