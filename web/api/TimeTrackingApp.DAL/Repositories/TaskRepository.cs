using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Repositories;

public sealed class TaskRepository(AppDbContext context) : BaseRepository<ProjectTaskEntity>(context), ITaskRepository
{
    public async Task<IReadOnlyList<ProjectTaskEntity>> GetAllWithProjectsAsync(CancellationToken ct = default)
        => await _dbSet.AsNoTrackingWithIdentityResolution().Include(t => t.Project).ToListAsync(ct);
}
