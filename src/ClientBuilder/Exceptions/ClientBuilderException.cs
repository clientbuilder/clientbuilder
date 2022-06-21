using System;

namespace ClientBuilder.Exceptions;

/// <summary>
/// Exception related to client builder.
/// </summary>
public class ClientBuilderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBuilderException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public ClientBuilderException(string message)
        : base(message)
    {
    }
}