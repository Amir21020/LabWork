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
}
