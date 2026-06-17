using Microsoft.Extensions.Logging;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.BL.Services;

public sealed class TimeEntryService(
    ITaskRepository taskRepository,
    ITimeEntryRepository timeEntryRepository,
    ILogger<TimeEntryService> logger) : ITimeEntryService
{
    public async Task CreateAsync(CreateTimeEntryRequest request, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Creating time entry: TaskId={TaskId}, Date={Date}, Hours={Hours}",
            request.TaskId, request.Date, request.Hours);

        var task = await taskRepository.GetByIdAsync(request.TaskId, ct);

        if (task is null)
        {
            logger.LogWarning("Task with Id={TaskId} not found for time entry", request.TaskId);
            throw new KeyNotFoundException($"Задача с Id {request.TaskId} не найдена.");
        }

        if (!task.IsActive)
        {
            logger.LogWarning("Task with Id={TaskId} is not active, cannot create time entry", request.TaskId);
            throw new InvalidOperationException("Нельзя создать проводку для неактивной задачи.");
        }

        var totalHoursByDay = await timeEntryRepository.GetDailyTotalHoursAsync(request.Date, ct);

        if (totalHoursByDay + request.Hours > 24)
        {
            logger.LogWarning(
                "Daily limit exceeded: Date={Date}, existing={ExistingHours}, requested={RequestedHours}",
                request.Date, totalHoursByDay, request.Hours);
            throw new InvalidOperationException($"Невозможно добавить {request.Hours} ч. Превышен суточный лимит времени (уже списано: {totalHoursByDay} ч.).");
        }

        var timeEntry = TimeEntryEntity.Create(request.Date, request.TaskId, request.Hours, request.Description);

        await timeEntryRepository.AddAsync(timeEntry, ct);

        logger.LogInformation(
            "Time entry created: Id={Id}, TaskId={TaskId}, Date={Date}, Hours={Hours}",
            timeEntry.Id, request.TaskId, request.Date, request.Hours);
    }

    public async Task UpdateAsync(Guid id, UpdateTimeEntryRequest request, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Updating time entry: Id={Id}, TaskId={TaskId}, Date={Date}, Hours={Hours}",
            id, request.TaskId, request.Date, request.Hours);

        var timeEntry = await timeEntryRepository.GetByIdAsync(id, ct);
        if (timeEntry is null)
        {
            logger.LogWarning("Time entry with Id={Id} not found for update", id);
            throw new KeyNotFoundException($"Проводка с Id {id} не найдена.");
        }

        var currentTask = await taskRepository.GetByIdAsync(timeEntry.TaskId, ct);
        if (currentTask is null)
        {
            logger.LogWarning("Task with Id={TaskId} not found for time entry update", timeEntry.TaskId);
            throw new KeyNotFoundException($"Задача с Id {timeEntry.TaskId} не найдена.");
        }

        if (!currentTask.IsActive && timeEntry.TaskId != request.TaskId)
        {
            logger.LogWarning(
                "Cannot change task for time entry Id={Id}: current task (Id={TaskId}) is inactive",
                id, timeEntry.TaskId);
            throw new InvalidOperationException("Нельзя изменить задачу для проводки, так как текущая задача неактивна.");
        }

        if (timeEntry.TaskId != request.TaskId)
        {
            var newTask = await taskRepository.GetByIdAsync(request.TaskId, ct);
            if (newTask is null)
            {
                logger.LogWarning("New task with Id={TaskId} not found for time entry update", request.TaskId);
                throw new KeyNotFoundException($"Задача с Id {request.TaskId} не найдена.");
            }

            if (!newTask.IsActive)
            {
                logger.LogWarning("New task with Id={TaskId} is not active, cannot assign to time entry", request.TaskId);
                throw new InvalidOperationException("Нельзя назначить проводку на неактивную задачу.");
            }
        }

        var totalHoursByDay = await timeEntryRepository.GetDailyTotalHoursAsync(request.Date, ct);
        totalHoursByDay -= timeEntry.Hours;

        if (totalHoursByDay + request.Hours > 24)
        {
            logger.LogWarning(
                "Daily limit exceeded on update: Date={Date}, existing={ExistingHours}, requested={RequestedHours}",
                request.Date, totalHoursByDay, request.Hours);
            throw new InvalidOperationException($"Невозможно обновить проводку: превышен суточный лимит времени (уже списано: {totalHoursByDay} ч.).");
        }

        timeEntry.Date = request.Date;
        timeEntry.Hours = request.Hours;
        timeEntry.Description = request.Description;
        timeEntry.TaskId = request.TaskId;

        await timeEntryRepository.UpdateAsync(timeEntry, ct);

        logger.LogInformation(
            "Time entry updated: Id={Id}, TaskId={TaskId}, Date={Date}, Hours={Hours}",
            id, request.TaskId, request.Date, request.Hours);
    }

    public async Task<IReadOnlyList<TimeEntryResponse>> GetListAsync(GetTimeEntriesRequest request, CancellationToken ct = default)
    {
        IReadOnlyList<TimeEntryEntity> entities;

        if (request.Date.HasValue)
        {
            entities = await timeEntryRepository.GetByDateAsync(request.Date.Value, ct);
        }
        else if (request.Month.HasValue && request.Year.HasValue)
        {
            entities = await timeEntryRepository.GetByMonthAsync(request.Month.Value, request.Year.Value, ct);
        }
        else
        {
            entities = await timeEntryRepository.GetAllAsync(ct);
        }

        return entities.Select(t => new TimeEntryResponse(
            t.Id,
            t.Date,
            t.Hours,
            t.Description ?? string.Empty,
            t.TaskId,
            t.Task?.Name ?? string.Empty)).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting time entry with Id={Id}", id);

        var timeEntry = await timeEntryRepository.GetByIdAsync(id, ct);
        if (timeEntry is null)
        {
            logger.LogWarning("Time entry with Id={Id} not found for deletion", id);
            return;
        }

        await timeEntryRepository.DeleteAsync(timeEntry, ct);

        logger.LogInformation("Time entry with Id={Id} deleted", id);
    }
}
