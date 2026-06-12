using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Interfaces;

public interface ITimeEntryRepository : IBaseRepository<TimeEntryEntity>
{
    Task<int> GetDailyTotalHoursAsync(DateOnly date, CancellationToken ct = default);
}
