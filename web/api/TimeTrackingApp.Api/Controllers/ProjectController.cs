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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> CreateAsync([FromBody] CreateProjectRequest request, CancellationToken ct = default)
    {
        await projectService.CreateAsync(request, ct);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct = default)
    {
        await projectService.UpdateAsync(id, request, ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await projectService.DeleteAsync(id, ct);

        return NoContent();
    }
}