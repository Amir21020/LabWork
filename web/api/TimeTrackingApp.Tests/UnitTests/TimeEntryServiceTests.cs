using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TimeTrackingApp.Api.Controllers;
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

    [Fact]
    public async Task Entries_are_retrieved_for_specific_date()
    {
        var taskRepoMock = new Mock<ITaskRepository>();
        var timeEntryRepoMock = new Mock<ITimeEntryRepository>();
        var sut = new TimeEntryService(taskRepoMock.Object, timeEntryRepoMock.Object, NullLogger<TimeEntryService>.Instance);

        var date = DateOnly.Parse("2026-06-01");
        var request = new GetTimeEntriesRequest(Date: date, Month: null, Year: null);

        timeEntryRepoMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeEntryEntity>
            {
                TimeEntryEntity.Create(date, Guid.NewGuid(), 4, "Filtered by date")
            });

        var result = await sut.GetListAsync(request);

        result.Should().HaveCount(1);
        result[0].Date.Should().Be(date);
        result[0].Hours.Should().Be(4);
        timeEntryRepoMock.Verify(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Entries_are_retrieved_for_entire_month()
    {
        var taskRepoMock = new Mock<ITaskRepository>();
        var timeEntryRepoMock = new Mock<ITimeEntryRepository>();
        var sut = new TimeEntryService(taskRepoMock.Object, timeEntryRepoMock.Object, NullLogger<TimeEntryService>.Instance);

        var request = new GetTimeEntriesRequest(Date: null, Month: 6, Year: 2026);

        timeEntryRepoMock
            .Setup(r => r.GetByMonthAsync(6, 2026, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeEntryEntity>
            {
                TimeEntryEntity.Create(DateOnly.Parse("2026-06-05"), Guid.NewGuid(), 2, "June entry")
            });

        var result = await sut.GetListAsync(request);

        result.Should().HaveCount(1);
        result[0].Hours.Should().Be(2);
        timeEntryRepoMock.Verify(r => r.GetByMonthAsync(6, 2026, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task All_entries_are_returned_when_year_is_missing_from_request()
    {
        var taskRepoMock = new Mock<ITaskRepository>();
        var timeEntryRepoMock = new Mock<ITimeEntryRepository>();
        var sut = new TimeEntryService(taskRepoMock.Object, timeEntryRepoMock.Object, NullLogger<TimeEntryService>.Instance);

        var request = new GetTimeEntriesRequest(Date: null, Month: 6, Year: null);

        timeEntryRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeEntryEntity>
            {
                TimeEntryEntity.Create(DateOnly.Parse("2026-06-01"), Guid.NewGuid(), 3, "Fallback")
            });

        var result = await sut.GetListAsync(request);

        result.Should().HaveCount(1);
        timeEntryRepoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        timeEntryRepoMock.Verify(r => r.GetByMonthAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Retrieved_entries_are_mapped_to_response_dto()
    {
        var taskRepoMock = new Mock<ITaskRepository>();
        var timeEntryRepoMock = new Mock<ITimeEntryRepository>();
        var sut = new TimeEntryService(taskRepoMock.Object, timeEntryRepoMock.Object, NullLogger<TimeEntryService>.Instance);

        var date = DateOnly.Parse("2026-06-10");
        var taskId = Guid.NewGuid();
        var entry = TimeEntryEntity.Create(date, taskId, 7, "Mapping test");
        entry.Id = Guid.NewGuid();

        timeEntryRepoMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeEntryEntity> { entry });

        var request = new GetTimeEntriesRequest(Date: date, Month: null, Year: null);
        var result = await sut.GetListAsync(request);

        var response = result.Should().ContainSingle().Subject;
        response.Id.Should().Be(entry.Id);
        response.Date.Should().Be(date);
        response.Hours.Should().Be(7);
        response.Description.Should().Be("Mapping test");
    }
}
