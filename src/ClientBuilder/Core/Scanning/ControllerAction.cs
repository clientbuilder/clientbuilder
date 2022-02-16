using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ClientBuilder.Common;

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
    /// Returns string that contains a list of all arguments for the current type based on type parameters.
    /// </summary>
    /// <param name="format">Format for rendering of each argument - {0} is type, {1} is name. Example: '{0} {1}'.</param>
    /// <param name="typeMapper"></param>
    /// <returns></returns>
    public string GetStronglyTypedClientArgumentListString(string format, IDictionary<string, string> typeMapper)
    {
        if (this.Arguments == null || !this.Arguments.Any())
        {
            return string.Empty;
        }

        return string.Join(", ", this.Arguments.Select(x => string.Format(format, x.Type.GetClientType(typeMapper), x.Name.ToFirstLower())));
    }

    /// <summary>
    /// Returns string that contains a list of all arguments for the current type based on type parameters.
    /// </summary>
    /// <returns></returns>
    public string GetClientArgumentNameListString()
    {
        if (this.Arguments == null || !this.Arguments.Any())
        {
            return string.Empty;
        }

        return string.Join(", ", this.Arguments.Select(x => x.Name.ToFirstLower()));
    }
}