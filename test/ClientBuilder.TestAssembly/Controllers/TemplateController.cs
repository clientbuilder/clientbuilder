using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[Route("/[controller]/")]
public class TemplateController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }
}