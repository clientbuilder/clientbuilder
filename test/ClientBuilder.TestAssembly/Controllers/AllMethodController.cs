using ClientBuilder.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[IncludeController("AllMethods")]
[Route("/all-method/")]
public class AllMethodController : Controller
{
    [IncludeAction]
    [HttpGet("get")]
    public IActionResult Get() => this.Ok();
    
    [IncludeAction]
    [HttpPost("post")]
    public IActionResult Post() => this.Ok();
    
    [IncludeAction]
    [HttpPut("put")]
    public IActionResult Put() => this.Ok();
    
    [IncludeAction]
    [HttpDelete("delete")]
    public IActionResult Delete() => this.Ok();
    
    [IncludeAction]
    [HttpPatch("patch")]
    public IActionResult Patch() => this.Ok();
    
    [IncludeAction]
    [HttpHead("head")]
    public IActionResult Head() => this.Ok();
    
    [IncludeAction]
    [HttpOptions("options")]
    public IActionResult Options() => this.Ok();
}