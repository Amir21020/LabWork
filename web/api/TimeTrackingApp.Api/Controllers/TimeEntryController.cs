using Microsoft.AspNetCore.Mvc;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;

namespace TimeTrackingApp.Api.Controllers;

public sealed class TimeEntryController(ITimeEntryService timeEntryService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTimeEntryRequest request, CancellationToken ct = default)
    {
        await timeEntryService.CreateAsync(request, ct);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TimeEntryResponse>>> GetListAsync([FromQuery] GetTimeEntriesRequest request, CancellationToken ct = default)
    {
        var result = await timeEntryService.GetListAsync(request, ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateTimeEntryRequest request, CancellationToken ct = default)
    {
        await timeEntryService.UpdateAsync(id, request, ct);
        return NoContent();
    }
}
