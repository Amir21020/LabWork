using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<GetTasksResponse>> GetAllAsync(CancellationToken ct = default);
}
