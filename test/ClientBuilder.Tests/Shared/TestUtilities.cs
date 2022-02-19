namespace ClientBuilder.Tests.Shared;

public static class TestUtilities
{
    public static string NormalizeJson(string jsonString)
    {
        return jsonString
            ?.Trim()
            .Replace("\r\n", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty)
            .Replace(" ", string.Empty);
    }
}