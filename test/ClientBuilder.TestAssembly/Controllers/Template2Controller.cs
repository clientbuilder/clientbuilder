using ClientBuilder.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[Route("/[controller]/[action]")]
[IncludeController]
public class Template2Controller : ControllerBase
{
    [IncludeAction]
    public IActionResult Index()
    {
        return Ok();
    }
}