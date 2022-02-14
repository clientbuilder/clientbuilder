using System;
using System.Collections.Generic;
using System.Linq;
using ClientBuilder.Common;
using ClientBuilder.Core.Modules;
using ClientBuilder.Exceptions;
using ClientBuilder.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientBuilder.Controllers;

/// <summary>
/// Client Builder API controller that provide all access and scaffold generation features.
/// </summary>
[Route("/_cb/api/scaffold/")]
[DisableCors]
public sealed class ScaffoldApiController : ControllerBase
{
    private readonly IScaffoldModuleRepository scaffoldModuleRepository;
    private readonly IScaffoldModuleGenerator scaffoldModuleGenerator;
    private readonly ILogger<ScaffoldApiController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldApiController"/> class.
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <param name="scaffoldModuleRepository"></param>
    /// <param name="scaffoldModuleGenerator"></param>
    /// <param name="logger"></param>
    public ScaffoldApiController(
        IWebHostEnvironment hostEnvironment,
        IScaffoldModuleRepository scaffoldModuleRepository,
        IScaffoldModuleGenerator scaffoldModuleGenerator,
        ILogger<ScaffoldApiController> logger)
    {
        this.scaffoldModuleRepository = scaffoldModuleRepository;
        this.scaffoldModuleGenerator = scaffoldModuleGenerator;
        this.logger = logger;
        if (!hostEnvironment.IsDevelopment())
        {
            throw new DevelopmentOnlyException("Client Builder cannot be accessed outside of development environment");
        }
    }

    /// <summary>
    /// Get all loaded scaffold modules.
    /// </summary>
    /// <returns></returns>
    [HttpGet("modules")]
    public IActionResult GetAllModules()
    {
        return this.Ok(this.scaffoldModuleRepository.GetModules().Select(ResponseMapper.MapToModel));
    }

    /// <summary>
    /// Trigger specified module generation.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("generate")]
    public IActionResult GenerateModule([FromBody]GenerationByIdRequest request)
    {
        var targetModule = this.scaffoldModuleRepository.GetModule(request.ModuleId);
        return this.TriggerGeneration(new List<ScaffoldModule> { targetModule });
    }

    /// <summary>
    /// Trigger generation for all modules that are filtered by instance type.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("generate/by-instance")]
    public IActionResult GenerateMobileModules([FromBody]GenerationByInstanceTypeRequest request)
    {
        var modules = this.scaffoldModuleRepository.GetModulesByInstance(request.InstanceType);
        return this.TriggerGeneration(modules);
    }

    /// <summary>
    /// Trigger generation for all modules that are filtered by client id.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("generate/by-client")]
    public IActionResult GenerateModulesByParentModuleId([FromBody]GenerationByClientIdRequest request)
    {
        var modules = this.scaffoldModuleRepository.GetModulesByClientId(request.ClientId);
        return this.TriggerGeneration(modules);
    }

    private IActionResult TriggerGeneration(IEnumerable<ScaffoldModule> modules)
    {
        try
        {
            var result = this.scaffoldModuleGenerator.Generate(modules.Select(x => x.Id));
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Client Builder generation failed");
            return this.BadRequest(ex.Message);
        }
    }
}