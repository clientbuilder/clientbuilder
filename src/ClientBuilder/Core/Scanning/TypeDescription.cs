using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientBuilder.Common;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Simplified assembly type description.
/// </summary>
public class TypeDescription
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDescription"/> class.
    /// </summary>
    public TypeDescription()
    {
        this.Properties = new List<PropertyDescription>();
    }

    /// <summary>
    /// Name of the class.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Full name of the class.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Full name of the class with the generic suffix.
    /// </summary>
    public string FullNameWithGeneric => this.FullName + this.GenericTypeSuffix;

    /// <summary>
    /// List of all properties of the class.
    /// </summary>
    public List<PropertyDescription> Properties { get; set; }

    /// <summary>
    /// Indicates that the type is collection or not.
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Indicates that the type is nullable or not.
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Indicates that the type is enumeration or not.
    /// </summary>
    public bool IsEnum { get; set; }

    /// <summary>
    /// Indicates that the type is generic type or not.
    /// </summary>
    public bool IsGenericType { get; set; }

    /// <summary>
    /// Generic type description of the type.
    /// </summary>
    public TypeDescription GenericType { get; set; }

    /// <summary>
    /// Generic type description suffix based on the <see cref="GenericType"/>.
    /// </summary>
    public string GenericTypeSuffix => (this.IsGenericType && this.GenericType != null) ? $"Of{this.GenericType?.Name}" : string.Empty;

    /// <summary>
    /// Indicates that the type is primitive type (false) or not (true).
    /// </summary>
    public bool IsComplex { get; set; }

    /// <summary>
    /// Enumeration values in case when type is enum.
    /// </summary>
    public Dictionary<string, int> EnumValues { get; set; }

    /// <summary>
    /// Gets client type based on specified type mapper.
    /// </summary>
    /// <param name="typeMapper"></param>
    /// <returns></returns>
    public string GetClientType(IDictionary<string, string> typeMapper) =>
        typeMapper.TryGetValue(this.Name, out var clientType) ? clientType : this.Name;

    /// <summary>
    /// Returns string that contains a list of all arguments for the current type based on type parameters.
    /// </summary>
    /// <param name="format">Format for rendering of each argument - {0} is type, {1} is name. Example: '{0} {1}'.</param>
    /// <param name="typeMapper"></param>
    /// <returns></returns>
    public string GetStronglyTypedClientArgumentListString(string format, IDictionary<string, string> typeMapper)
    {
        if (this.Properties == null || this.Properties.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(", ", this.Properties.Select(x => string.Format(format, x.Type.GetClientType(typeMapper), x.Name.ToFirstLower())));
    }

    /// <summary>
    /// Returns string that contains a list of all arguments for the current type based on type parameters.
    /// </summary>
    /// <returns></returns>
    public string GetClientArgumentNameListString()
    {
        if (this.Properties == null || this.Properties.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(", ", this.Properties.Select(x => x.Name.ToFirstLower()));
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Name;
    }
}