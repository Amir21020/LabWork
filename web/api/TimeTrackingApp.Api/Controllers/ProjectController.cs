using Microsoft.AspNetCore.Mvc;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;

namespace TimeTrackingApp.Api.Controllers;

public sealed class ProjectController(IProjectService projectService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GetProjectsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GetProjectsResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var projects = await projectService.GetAllAsync(ct);

        return Ok(projects);
    }
}