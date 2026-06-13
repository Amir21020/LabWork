using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Interfaces;

public interface ITimeEntryService
{
    Task CreateAsync(CreateTimeEntryRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TimeEntryResponse>> GetListAsync(GetTimeEntriesRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateTimeEntryRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
