namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Implementation of enumeration that describe the enumeration item with its name and value, and helper key (standard SNAKE_UPPER_CASE).
/// </summary>
public record EnumValueItem
{
    /// <summary>
    /// Name of the enumeration item.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Original name of the enumeration item.
    /// </summary>
    public string OriginalName { get; internal set; }

    /// <summary>
    /// Value of the enumeration item.
    /// </summary>
    public int Value { get; internal set; }

    /// <summary>
    /// The additional key of the enumeration item.
    /// </summary>
    public string Key { get; internal set; }
}