using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.Tests.IntegrationTests;

[Collection("ProjectsCollection")]
public sealed class TimeEntryControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task DisposeAsync() => factory.ResetDatabaseAsync();
    public Task InitializeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Creating_time_entry_succeeds_and_persists_data()
    {
        var taskId = await CreateActiveTaskAsync();
        var date = DateOnly.FromDateTime(DateTime.Today);

        var request = new CreateTimeEntryRequest(date, 3, "Work description", taskId);
        var response = await factory.HttpClient.PostAsJsonAsync("/api/timeentry", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entries = dbContext.TimeEntries
            .Where(e => e.TaskId == taskId && e.Date == date)
            .ToList();

        entries.Should().HaveCount(1);
        entries[0].Hours.Should().Be(3);
        entries[0].Description.Should().Be("Work description");
        entries[0].TaskId.Should().Be(taskId);
        entries[0].Date.Should().Be(date);
    }

    [Fact]
    public async Task Creating_time_entry_fails_when_task_does_not_exist()
    {
        var request = new CreateTimeEntryRequest(DateOnly.FromDateTime(DateTime.Today), 4, "Test", Guid.NewGuid());
        var response = await factory.HttpClient.PostAsJsonAsync("/api/timeentry", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreateActiveTaskAsync()
    {
        var projectId = await CreateProjectAsync();
        return await CreateTaskAsync(projectId, "Active Task", true);
    }

    private async Task<Guid> CreateProjectAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var project = ProjectEntity.Create("Test Project", "TST", true);
        await dbContext.AddAsync(project);
        await dbContext.SaveChangesAsync();

        return project.Id;
    }

    private async Task<Guid> CreateTaskAsync(Guid projectId, string name, bool isActive)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var task = ProjectTaskEntity.Create(name, projectId, isActive);
        await dbContext.AddAsync(task);
        await dbContext.SaveChangesAsync();

        return task.Id;
    }
}
