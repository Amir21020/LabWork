using Microsoft.AspNetCore.Mvc;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;

namespace TimeTrackingApp.Api.Controllers;

public sealed class TaskController(ITaskService taskService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GetTasksResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GetTasksResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var tasks = await taskService.GetAllAsync(ct);
        return Ok(tasks);
    }
}
