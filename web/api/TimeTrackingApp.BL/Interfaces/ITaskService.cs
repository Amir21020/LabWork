using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<GetTasksResponse>> GetAllAsync(CancellationToken ct = default);
    Task CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default);
}