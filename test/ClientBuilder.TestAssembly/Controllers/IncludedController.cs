using System.Collections.Generic;
using ClientBuilder.DataAnnotations;
using ClientBuilder.TestAssembly.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[IncludeController]
[Route("/api/main/")]
public class IncludedController : Controller
{
    [IncludeAction(typeof(IEnumerable<SomeModel>))]
    [HttpGet("data")]
    public IActionResult Data() => this.Ok(new List<SomeModel>());

    [IncludeAction(typeof(bool))]
    [HttpPost]
    [Route("data")]
    public IActionResult AddData() => this.Ok(true);

    [IncludeAction]
    [HttpPost("check")]
    public IActionResult Check() => this.Ok();
}