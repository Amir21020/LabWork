using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.DAL.Repositories;

public sealed class TimeEntryRepository(AppDbContext context) : BaseRepository<TimeEntryEntity>(context), ITimeEntryRepository
{
    public async Task<int> GetDailyTotalHoursAsync(DateOnly date, CancellationToken ct = default)
        => await _dbSet.Where(t => t.Date == date).SumAsync(t => t.Hours, ct);
}
