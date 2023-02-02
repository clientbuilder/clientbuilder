using System;
using System.Collections.Generic;
using System.Linq;
using ClientBuilder.Attributes;
using ClientBuilder.Extensions;
using Microsoft.Extensions.Logging;

namespace ClientBuilder.Core.Scanning;

/// <inheritdoc />
public class SourceRepository : ISourceRepository
{
    private readonly IAssemblyScanner assemblyScanner;
    private readonly IDescriptionExtractor descriptionExtractor;
    private readonly ILogger<SourceRepository> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceRepository"/> class.
    /// </summary>
    /// <param name="assemblyScanner"></param>
    /// <param name="descriptionExtractor"></param>
    /// <param name="logger"></param>
    public SourceRepository(
        IAssemblyScanner assemblyScanner,
        IDescriptionExtractor descriptionExtractor,
        ILogger<SourceRepository> logger)
    {
        this.assemblyScanner = assemblyScanner;
        this.descriptionExtractor = descriptionExtractor;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> Fetch(Func<SourceAssemblyType, bool> filter) =>
        this.FetchDescriptions(filter);

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> FetchEnums(Func<SourceAssemblyType, bool> filter = null) =>
        this.FetchDescriptions(x => x.Type.IsEnum, filter);

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> FetchIncludedEnums(Func<SourceAssemblyType, bool> filter = null) =>
        this.FetchDescriptions(x => x.Type.IsEnum && x.Type.HasCustomAttribute<IncludeElementAttribute>(), filter);

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> FetchIncludedClasses(Func<SourceAssemblyType, bool> filter = null) =>
        this.FetchDescriptions(x => x.Type.IsClass && x.Type.HasCustomAttribute<IncludeElementAttribute>(), filter);

    private IEnumerable<TypeDescription> FetchDescriptions(
        Func<SourceAssemblyType, bool> primaryFilter,
        Func<SourceAssemblyType, bool> secondaryFilter = null)
    {
        try
        {
            Func<SourceAssemblyType, bool> additionalFilter = secondaryFilter ?? (_ => true);
            var enumsTypeDescriptions = this.assemblyScanner
                .FetchSourceTypes()
                .Where(primaryFilter)
                .Where(additionalFilter)
                .Select(x => this.descriptionExtractor.ExtractTypeDescription(x.Type))
                .ToList();

            return enumsTypeDescriptions;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred during fetching types");
            return new List<TypeDescription>();
        }
    }
}