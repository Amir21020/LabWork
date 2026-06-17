using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.DAL.Repositories;

public sealed class TimeEntryRepository(AppDbContext context) : BaseRepository<TimeEntryEntity>(context), ITimeEntryRepository
{
    public override async Task<IReadOnlyList<TimeEntryEntity>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.Include(te => te.Task).ToListAsync(ct);

    public async Task<IReadOnlyList<TimeEntryEntity>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => await _dbSet
        .Include(te => te.Task)
        .Where(te => te.Date == date)
        .ToListAsync(ct);

    public async Task<IReadOnlyList<TimeEntryEntity>> GetByMonthAsync(int month, int year, CancellationToken ct = default)
        => await _dbSet.Include(te => te.Task).Where(t => t.Date.Month == month && t.Date.Year == year).ToListAsync(ct);

    public async Task<int> GetDailyTotalHoursAsync(DateOnly date, CancellationToken ct = default)
        => await _dbSet.Where(t => t.Date == date).SumAsync(t => t.Hours, ct);
}
