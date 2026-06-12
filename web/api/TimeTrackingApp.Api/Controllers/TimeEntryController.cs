using Microsoft.AspNetCore.Mvc;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;

namespace TimeTrackingApp.Api.Controllers;

public sealed class TimeEntryController(ITimeEntryService timeEntryService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateAsync(CreateTimeEntryRequest request, CancellationToken ct = default)
    {
        await timeEntryService.CreateAsync(request, ct);
        return NoContent();
    }
}
