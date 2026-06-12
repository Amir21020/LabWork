using Microsoft.Extensions.Logging;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.DAL.Repositories;

namespace TimeTrackingApp.BL.Services;

public sealed class TaskService(ILogger<TaskService> logger, ITaskRepository taskRepository) : ITaskService
{
    public async Task<IReadOnlyList<GetTasksResponse>> GetAllAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Fetching all tasks");

        var tasks = await taskRepository.GetAllWithProjectsAsync(ct);

        return tasks
            .Select(t => new GetTasksResponse(t.Id, t.Name, t.Project.Id, t.Project.Name, t.IsActive))
            .ToList();
    }
}