using Microsoft.AspNetCore.Mvc;

namespace ClientBuilder.TestAssembly.Controllers;

[Route("sample")]
public class SampleController
{
    [HttpGet("sample/get")]
    public void GetAction() { }

    [HttpPost("sample/post")]
    public void PostAction() { }

    public void NoRouteAction() { }
}