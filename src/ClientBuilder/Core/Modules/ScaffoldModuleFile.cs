﻿using System.Collections.Generic;

namespace ClientBuilder.Core.Modules;

/// <summary>
/// Implementation of a file which will be generated by the Client Builder.
/// </summary>
public class ScaffoldModuleFile
{
    /// <summary>
    /// Name of the file with extension.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Relative path of the file.
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Unique identifier of the file in case of generation files with same layouts and different parameters.
    /// </summary>
    public string ReferenceId { get; set; }

    /// <summary>
    /// Template type that must be used for creating the content of the file.
    /// </summary>
    public IFileTemplate Template { get; set; }

    /// <summary>
    /// Data related to the file itself.
    /// </summary>
    public object ContextData { get; set; }

    /// <summary>
    /// Flag that indicates whether the module generated file is locked for changes or not.
    /// </summary>
    public bool Locked { get; set; } = true;

    /// <summary>
    /// Builds content of the file content.
    /// </summary>
    /// <returns></returns>
    public string BuildContent() => this.Template.Render(this.ContextData);
}