using ClientBuilder.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[IncludeController]
public class QuickController : ControllerBase
{
    [HttpGet]
    [IncludeAction]
    public IActionResult Index()
    {
        return this.Ok();
    }
}