using ClientBuilder.DataAnnotations;
using ClientBuilder.TestAssembly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[IncludeController("Private", "Main")]
[Route("/api/secondary/")]
public class SecondIncludedController : Controller
{
    [IncludeAction(typeof(SomeModel))]
    [HttpGet("data/{id}")]
    [Authorize]
    public IActionResult DataItem(string id) => this.Ok(new SomeModel());
}