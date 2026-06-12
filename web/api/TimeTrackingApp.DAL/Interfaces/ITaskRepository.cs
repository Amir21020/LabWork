using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Interfaces;

public interface ITaskRepository : IBaseRepository<ProjectTaskEntity>
{
    Task<IReadOnlyList<ProjectTaskEntity>> GetAllWithProjectsAsync(CancellationToken ct = default);
}
