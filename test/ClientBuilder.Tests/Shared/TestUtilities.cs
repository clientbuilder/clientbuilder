using System;
using Microsoft.Extensions.Logging;
using Moq;

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
    
    public static Mock<ILogger<T>> VerifyDebugWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
    {
        Func<object, Type, bool> state = (v, t) => v.ToString().CompareTo(expectedMessage) == 0;
    
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

        return logger;
    }
}