using ClientBuilder.DataAnnotations;
using ClientBuilder.TestAssembly.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[IncludeController]
[Route("/api/secondary/")]
public class SecondIncludedController : Controller
{
    [IncludeAction(typeof(SomeModel))]
    [HttpGet("data/{id}")]
    public IActionResult DataItem(string id) => this.Ok(new SomeModel());
}