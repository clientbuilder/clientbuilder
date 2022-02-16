using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// File template based on JSON serialization.
/// </summary>
public class JsonFileTemplate : IFileTemplate
{
    private readonly JsonSerializerSettings settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileTemplate"/> class.
    /// </summary>
    /// <param name="settings"></param>
    public JsonFileTemplate(JsonSerializerSettings settings = null)
    {
        if (settings != null)
        {
            this.settings = settings;
        }
        else
        {
            DefaultContractResolver contractResolver = new ()
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };

            this.settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
            };
        }
    }

    /// <inheritdoc/>
    public string Render(object contextData)
    {
        return JsonConvert.SerializeObject(contextData, this.settings);
    }
}