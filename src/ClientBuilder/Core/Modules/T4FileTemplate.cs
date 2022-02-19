using System;
using System.Collections.Generic;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// File template based on T4 text rendering. Please consider that the render's context data must be a
/// <see cref="IDictionary{TKey,TValue}"/> (T4 session) with string key and object value.
/// </summary>
public class T4FileTemplate : IFileTemplate
{
    private readonly Type templateType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T4FileTemplate"/> class.
    /// </summary>
    /// <param name="templateType"></param>
    public T4FileTemplate(Type templateType)
    {
        this.templateType = templateType;
    }

    /// <inheritdoc/>
    public string Render(object contextData)
    {
        var contextDataDictionary = (IDictionary<string, object>)contextData;
        var templateInstance = Activator.CreateInstance(this.templateType);
        this.templateType.GetProperty("Session")?.SetValue(templateInstance, contextData);
        object templateContentObject = this.templateType.GetMethod("TransformText")?.Invoke(templateInstance, null);

        return templateContentObject?.ToString();
    }
}