using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Interfaces;

public interface ITimeEntryService
{
    Task CreateAsync(CreateTimeEntryRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TimeEntryResponse>> GetListAsync(GetTimeEntriesRequest request, CancellationToken ct = default);
}
