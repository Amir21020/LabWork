using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Interfaces;

public interface IProjectRepository : IBaseRepository<ProjectEntity>
{
    Task<bool> ExistsAsync(Guid id,  CancellationToken cancellationToken = default);
}
