﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Exceptions;
using ClientBuilder.RuleSet;

namespace ClientBuilder.Options;

/// <summary>
/// Client Builder options class that contains all configuration capabilities.
/// </summary>
public class ClientBuilderOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBuilderOptions"/> class.
    /// </summary>
    public ClientBuilderOptions()
    {
        this.Assemblies = new List<Assembly>();
        this.ScanningRules = new List<IScanningRules>();
        this.ModulesTypes = new List<Type>();
        this.Clients = new List<ClientOptions>();
        this.PrimitiveTypes = new Dictionary<Type, string>(ClientBuilderDefaults.PrimitiveTypes);

        this.InitializeDefaults();
    }

    /// <summary>
    /// List of all assemblies that are going to be scanned for the purposes of client builder.
    /// </summary>
    public IList<Assembly> Assemblies { get; }

    /// <summary>
    /// List of all rule sets used as a strategy for assembly scanning.
    /// </summary>
    public IList<IScanningRules> ScanningRules { get; }

    /// <summary>
    /// List of all scaffold modules used for code generation from the Client Builder.
    /// </summary>
    public List<Type> ModulesTypes { get; }

    /// <summary>
    /// Map between primitive types and their names. That map is used to define which types can be used
    /// as primitive from the Client Builder. If a primitive type is missing in this dictionary
    /// the engine will define the type as non primitive.
    /// </summary>
    public IDictionary<Type, string> PrimitiveTypes { get; }

    /// <summary>
    /// Collection of all registered clients.
    /// </summary>
    public IList<ClientOptions> Clients { get; }

    /// <summary>
    /// The content root path of the main application used for assembly scanning (ASP.NET app).
    /// </summary>
    public string ContentRootPath { get; set; }

    /// <summary>
    /// Method that set the mobile application path into the options.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientName"></param>
    /// <param name="amountDirectoriesBack"></param>
    /// <param name="paths">Paths are defined by the solution folder.</param>
    public void AddClient(string clientId, string clientName, ushort amountDirectoriesBack, params string[] paths)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentNullException(clientId);
        }

        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new ArgumentNullException(clientName);
        }

        if (this.Clients.Any(x => x.Id?.Equals(clientId, StringComparison.InvariantCultureIgnoreCase) ?? false))
        {
            throw new ClientBuilderException($"Client with ID: '{clientId}' already exists");
        }

        var pathSegments = new List<string>();
        pathSegments.AddRange(Enumerable.Repeat("..", amountDirectoriesBack));
        pathSegments.AddRange(paths);
        this.Clients.Add(new ClientOptions
        {
            Id = clientId,
            Name = clientName,
            Path = Path.Combine(pathSegments.ToArray()),
        });
    }

    /// <summary>
    /// Returns client options for a specified identifier.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public ClientOptions GetClient(string clientId)
    {
        var client = this.Clients.FirstOrDefault(x =>
            x.Id.Equals(clientId, StringComparison.InvariantCultureIgnoreCase));
        if (client == null)
        {
            throw new ClientBuilderException($"There is no defined client with ID: '{clientId}'");
        }

        return client;
    }

    /// <summary>
    /// Adds an assembly to the <see cref="Assemblies"/>.
    /// </summary>
    /// <param name="assemblyString"></param>
    public void AddAssembly(string assemblyString)
    {
        this.AddAssembly(Assembly.Load(assemblyString));
    }

    /// <summary>
    /// Adds an assembly to the <see cref="Assemblies"/>.
    /// </summary>
    /// <param name="assembly"></param>
    public void AddAssembly(Assembly assembly)
    {
        this.Assemblies.Add(assembly);
    }

    /// <summary>
    /// Adds a scanning rules to the <see cref="ScanningRules"/>.
    /// </summary>
    /// <param name="rules"></param>
    public void AddScanningRules(IScanningRules rules)
    {
        this.ScanningRules.Add(rules);
    }

    /// <summary>
    /// Add scaffold module to the Client Builder storage.
    /// </summary>
    /// <typeparam name="TModule">Type of the module.</typeparam>
    public void AddModule<TModule>()
        where TModule : ScaffoldModule
    {
        this.ModulesTypes.Add(typeof(TModule));
    }

    private void InitializeDefaults()
    {
        this.AddScanningRules(new DefaultControllersScanningRules());
        this.AddScanningRules(new DefaultElementsScanningRules());
    }
}