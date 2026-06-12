using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.DAL.Repositories;

public sealed class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(p => p.Id == id, cancellationToken);
}
