using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ClientBuilder.Core.Scanning;

/// <summary>
/// Model that defines a controller action exposed for the needs of Client Builder.
/// </summary>
public class ControllerAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControllerAction"/> class.
    /// </summary>
    public ControllerAction()
    {
        this.Arguments = new List<ArgumentDescription>();
    }

    /// <summary>
    /// Identification of the action.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Name of the controller which contains the action.
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// Action method name.
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// Route of the action.
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// Is the action requires authentication to be executed.
    /// </summary>
    public bool Authorized { get; set; }

    /// <summary>
    /// Description if the response of the action.
    /// </summary>
    public ResponseDescription Response { get; set; }

    /// <summary>
    /// <see cref="HttpMethod"/> of the action.
    /// </summary>
    public HttpMethod Method { get; set; }

    /// <summary>
    /// HTTP method name of the action.
    /// </summary>
    public string MethodName => this.Method?.Method;

    /// <summary>
    /// Arguments of the action.
    /// </summary>
    public IEnumerable<ArgumentDescription> Arguments { get; set; }

    /// <summary>
    /// The complex argument of the action. The purpose of this property is to get the main request object of the request.
    /// </summary>
    public ArgumentDescription ComplexArgument => this.Arguments.FirstOrDefault(x => x.Type.IsComplex);

    /// <summary>
    /// Arguments names of the action, separated with comma and join into a string.
    /// </summary>
    public string ArgumentsListString
    {
        get
        {
            if (this.Arguments == null || !this.Arguments.Any())
            {
                return string.Empty;
            }

            return string.Join(", ", this.Arguments.Select(x => x.Name));
        }
    }

    /// <summary>
    /// Arguments names of the action, separated with comma and join into a string with their types.
    /// </summary>
    public string StrongTypedArgumentsListString
    {
        get
        {
            if (this.Arguments == null || !this.Arguments.Any())
            {
                return string.Empty;
            }

            return string.Join(", ", this.Arguments.Select(x => $"{x.Type.Name} {x.Name}"));
        }
    }

    /// <summary>
    /// Arguments names of the action in client format, separated with comma and join into a string with their types.
    /// The default definition consider client as JavaScript.
    /// </summary>
    public string StrongTypedClientArgumentsListString
    {
        get
        {
            if (this.Arguments == null || !this.Arguments.Any())
            {
                return string.Empty;
            }

            return string.Join(", ", this.Arguments.Select(x => $"{x.Name}: {x.Type.ClientTypeName}"));
        }
    }
}