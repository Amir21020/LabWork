using Microsoft.Extensions.Logging;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.BL.Services;

public sealed class TaskService(ILogger<TaskService> logger, IProjectRepository projectRepository, IBaseRepository<ProjectTaskEntity> taskRepository) : ITaskService
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
        await taskRepository.AddAsync(task, ct);

        logger.LogInformation("Task with Id={Id} created", task.Id);
    }

    public async Task<IReadOnlyList<GetTasksResponse>> GetAllAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Fetching all tasks");

        var tasks = await taskRepository.GetAllAsync(ct);

        return tasks
            .Select(t => new GetTasksResponse(t.Id, t.Name, t.Project.Id, t.Project.Name, t.IsActive))
            .ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting task with Id={Id}", id);

        var task = await taskRepository.GetByIdAsync(id, ct);

        if (task is null)
        {
            logger.LogWarning("Task with Id={Id} not found for deletion", id);
            return;
        }

        await taskRepository.DeleteAsync(task, ct);

        logger.LogInformation("Task with Id={Id} deleted", id);
    }

    public async Task UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default)
    {
        logger.LogInformation("Updating task with Id={Id}", id);

        var task = await taskRepository.GetByIdAsync(id, ct);

        if (task is null)
        {
            logger.LogWarning("Task with Id={Id} not found for update", id);
            return;
        }

        task.Name = request.Name;
        task.IsActive = request.IsActive;

        await taskRepository.UpdateAsync(task, ct);

        logger.LogInformation("Task with Id={Id} updated", id);
    }

}