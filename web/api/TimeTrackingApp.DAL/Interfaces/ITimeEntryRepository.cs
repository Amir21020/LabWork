using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Interfaces;

public interface ITimeEntryRepository : IBaseRepository<TimeEntryEntity>
{
    Task<int> GetDailyTotalHoursAsync(DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<TimeEntryEntity>> GetByMonthAsync(int month, int year, CancellationToken ct = default);
    Task<IReadOnlyList<TimeEntryEntity>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}
