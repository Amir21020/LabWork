using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Services;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.Tests.UnitTests;

public sealed class TimeEntryServiceTests
{
    [Fact]
    public async Task Creation_fails_when_task_does_not_exist()
    {
        var taskRepositoryMock = new Mock<ITaskRepository>();
        var timeEntryRepositoryMock = new Mock<ITimeEntryRepository>();

        var sut = new TimeEntryService(
            taskRepositoryMock.Object,
            timeEntryRepositoryMock.Object,
            NullLogger<TimeEntryService>.Instance);

        var request = new CreateTimeEntryRequest(DateOnly.FromDateTime(DateTime.Today), 4, "", Guid.NewGuid());

        taskRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectTaskEntity?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.CreateAsync(request));

        timeEntryRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TimeEntryEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Creation_fails_when_task_is_inactive()
    {
        var taskRepositoryMock = new Mock<ITaskRepository>();
        var timeEntryRepositoryMock = new Mock<ITimeEntryRepository>();

        var sut = new TimeEntryService(
            taskRepositoryMock.Object,
            timeEntryRepositoryMock.Object,
            NullLogger<TimeEntryService>.Instance);

        var request = new CreateTimeEntryRequest(DateOnly.FromDateTime(DateTime.Today), 4, "", Guid.NewGuid());

        var inactiveTask = ProjectTaskEntity.Create(It.IsAny<String>(), Guid.NewGuid(), false);

        taskRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveTask);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreateAsync(request));

        timeEntryRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TimeEntryEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Creation_fails_when_daily_limit_is_exceeded()
    {
        var taskRepositoryMock = new Mock<ITaskRepository>();
        var timeEntryRepositoryMock = new Mock<ITimeEntryRepository>();

        var sut = new TimeEntryService(
            taskRepositoryMock.Object,
            timeEntryRepositoryMock.Object,
            NullLogger<TimeEntryService>.Instance);

        var request = new CreateTimeEntryRequest(DateOnly.FromDateTime(DateTime.Today), 5, "", Guid.NewGuid());


        var activeTask = ProjectTaskEntity.Create("", Guid.NewGuid(), true);
        
        taskRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTask);
        
        timeEntryRepositoryMock
            .Setup(r => r.GetDailyTotalHoursAsync(request.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(20);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreateAsync(request));

        timeEntryRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TimeEntryEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Time_entry_is_successfully_created()
    {
        var taskRepositoryMock = new Mock<ITaskRepository>();
        var timeEntryRepositoryMock = new Mock<ITimeEntryRepository>();

        var sut = new TimeEntryService(
            taskRepositoryMock.Object,
            timeEntryRepositoryMock.Object,
            NullLogger<TimeEntryService>.Instance);

        var taskId = Guid.NewGuid();
        var request = new CreateTimeEntryRequest(DateOnly.FromDateTime(DateTime.Today), 4, "Разработка нового функционала", taskId);

        var activeTask = ProjectTaskEntity.Create("Test Task", Guid.NewGuid(), true);
        activeTask.Id = taskId;

        taskRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTask);

        timeEntryRepositoryMock
            .Setup(r => r.GetDailyTotalHoursAsync(request.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(8);

        await sut.CreateAsync(request);

        timeEntryRepositoryMock.Verify(
            r => r.AddAsync(It.Is<TimeEntryEntity>(entry =>
                entry.TaskId == request.TaskId &&
                entry.Date == request.Date &&
                entry.Hours == request.Hours &&
                entry.Description == request.Description),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

}
