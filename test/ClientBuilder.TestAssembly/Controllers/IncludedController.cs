using System.Collections.Generic;
using ClientBuilder.DataAnnotations;
using ClientBuilder.TestAssembly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[IncludeController(Groups = new [] { "Main" })]
[Route("/api/main/")]
[Authorize]
public class IncludedController : Controller
{
    [IncludeAction(typeof(IEnumerable<SomeModel>))]
    [HttpGet("data")]
    [AllowAnonymous]
    public IActionResult Data() => this.Ok(new List<SomeModel>());

    [IncludeAction(typeof(bool))]
    [HttpPost]
    [Route("data")]
    public IActionResult AddData(SomeModel model) => this.Ok(true);

    [IncludeAction]
    [HttpPost("check")]
    public IActionResult Check() => this.Ok();
}