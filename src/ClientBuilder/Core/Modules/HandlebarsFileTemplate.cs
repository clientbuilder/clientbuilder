using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// File template based on Handlebars templates.
/// </summary>
public class HandlebarsFileTemplate : IFileTemplate
{
    private readonly HandlebarsTemplate<object, object> template;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlebarsFileTemplate"/> class.
    /// </summary>
    /// <param name="templatePath"></param>
    public HandlebarsFileTemplate(string templatePath)
    {
        var templateContent = File.ReadAllText(templatePath);
        this.template = Handlebars.Compile(templateContent);
    }

    /// <inheritdoc/>
    public string Render(object contextData)
    {
        return this.template(contextData);
    }
}