using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HandlebarsDotNet;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// File template based on Handlebars templates.
/// For more information about the template you can check -> https://handlebarsjs.com/.
/// </summary>
public class HandlebarsFileTemplate : IFileTemplate
{
    private readonly HandlebarsTemplate<object, object> template;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlebarsFileTemplate"/> class.
    /// </summary>
    /// <param name="templatePaths"></param>
    public HandlebarsFileTemplate(params string[] templatePaths)
    {
        var baseProjectPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
        var allTemplatePaths = new List<string> { baseProjectPath };
        allTemplatePaths.AddRange(templatePaths);
        var fullTemplatePath = Path.Combine(allTemplatePaths.ToArray());
        var templateContent = File.ReadAllText(fullTemplatePath);

        this.template = Handlebars.Compile(templateContent);
    }

    /// <inheritdoc/>
    public string Render(object contextData)
    {
        return this.template(contextData);
    }
}