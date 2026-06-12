using Microsoft.Extensions.Logging;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.BL.Services;

public sealed class TaskService(ILogger<TaskService> logger, IProjectRepository projectRepository, ITaskRepository taskRepository) : ITaskService
{
    public async Task CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        logger.LogInformation("Creating task with Name={Name}, ProjectId={ProjectId}", request.Name, request.ProjectId);

        bool projectExists = await projectRepository.ExistsAsync(request.ProjectId);
        if (!projectExists)
        {
            logger.LogWarning("Project with Id={ProjectId} not found for task creation", request.ProjectId);
            throw new ArgumentException($"Проект с ID {request.ProjectId} не найден.");
        }

        var task = ProjectTaskEntity.Create(request.Name, request.ProjectId, request.IsActive);
        await taskRepository.AddAsync(task);

        logger.LogInformation("Task with Id={Id} created", task.Id);
    }

    public async Task<IReadOnlyList<GetTasksResponse>> GetAllAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Fetching all tasks");

        var tasks = await taskRepository.GetAllWithProjectsAsync(ct);

        return tasks
            .Select(t => new GetTasksResponse(t.Id, t.Name, t.Project.Id, t.Project.Name, t.IsActive))
            .ToList();
    }
}