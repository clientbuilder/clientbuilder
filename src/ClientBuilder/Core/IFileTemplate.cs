using System.Collections.Generic;

namespace ClientBuilder.Core;

/// <summary>
/// A contract that defines the file template and its render strategy.
/// </summary>
public interface IFileTemplate
{
    /// <summary>
    /// Renders content by using required context data.
    /// </summary>
    /// <param name="contextData"></param>
    /// <returns></returns>
    string Render(IDictionary<string, object> contextData);
}