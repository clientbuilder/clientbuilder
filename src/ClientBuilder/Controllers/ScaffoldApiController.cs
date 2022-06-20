using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
[EnableCors(Constants.ClientBuilderCorsPolicy)]
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
    /// Checks whether the application is available.
    /// </summary>
    /// <returns></returns>
    [HttpPost("check")]
    public IActionResult CheckAvailability()
    {
        return this.Ok();
    }

    /// <summary>
    /// Get all loaded scaffold modules.
    /// </summary>
    /// <returns></returns>
    [HttpGet("modules")]
    public async Task<IActionResult> GetAllModules()
    {
        var modules = await this.scaffoldModuleRepository.GetModulesAsync();
        return this.Ok(modules.Select(ResponseMapper.MapToModel));
    }

    /// <summary>
    /// Trigger specified module generation.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateModule([FromBody]GenerationByIdRequest request)
    {
        var modulesForGeneration = new List<ScaffoldModule>();
        if (string.IsNullOrWhiteSpace(request.ModuleId))
        {
            var modules = await this.scaffoldModuleRepository.GetModulesAsync();
            modulesForGeneration.AddRange(modules);
        }
        else
        {
            var targetModule = await this.scaffoldModuleRepository.GetModuleAsync(request.ModuleId);
            modulesForGeneration.Add(targetModule);
        }

        return await this.TriggerGenerationAsync(modulesForGeneration);
    }

    /// <summary>
    /// Trigger generation for all modules that are filtered by instance type.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("generate/by-instance")]
    public async Task<IActionResult> GenerateModulesByInstance([FromBody]GenerationByInstanceTypeRequest request)
    {
        var modules = await this.scaffoldModuleRepository.GetModulesByInstanceAsync(request.InstanceType);
        return await this.TriggerGenerationAsync(modules);
    }

    /// <summary>
    /// Trigger generation for all modules that are filtered by client id.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("generate/by-client")]
    public async Task<IActionResult> GenerateModulesByClientId([FromBody]GenerationByClientIdRequest request)
    {
        var modules = await this.scaffoldModuleRepository.GetModulesByClientIdAsync(request.ClientId);
        return await this.TriggerGenerationAsync(modules);
    }

    private async Task<IActionResult> TriggerGenerationAsync(IEnumerable<ScaffoldModule> modules)
    {
        try
        {
            var result = await this.scaffoldModuleGenerator.GenerateAsync(modules.Select(x => x.Id));
            return this.Ok(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Client Builder generation failed");
            return this.BadRequest(ex.Message);
        }
    }
}