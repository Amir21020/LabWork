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

    [Fact]
    public async Task Querying_time_entries_returns_empty_list_when_there_are_none()
    {
        var response = await factory.HttpClient.GetAsync("/api/timeentry");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var entries = await response.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();
        entries.Should().BeEmpty();
    }

    [Fact]
    public async Task Querying_time_entries_returns_all_entries()
    {
        var taskId = await CreateActiveTaskAsync();
        var date = DateOnly.FromDateTime(DateTime.Today);

        var req1 = new CreateTimeEntryRequest(date, 3, "First", taskId);
        var req2 = new CreateTimeEntryRequest(date, 2, "Second", taskId);

        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", req1);
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", req2);

        var response = await factory.HttpClient.GetAsync("/api/timeentry");
        var entries = await response.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();

        entries.Should().HaveCount(2);
        entries.Should().Contain(e => e.Hours == 3 && e.Description == "First");
        entries.Should().Contain(e => e.Hours == 2 && e.Description == "Second");
    }

    [Fact]
    public async Task Querying_time_entries_filters_by_date()
    {
        var taskId = await CreateActiveTaskAsync();
        var date1 = DateOnly.FromDateTime(DateTime.Today);
        var date2 = date1.AddDays(1);

        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date1, 4, "Today", taskId));
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date2, 5, "Tomorrow", taskId));

        var response = await factory.HttpClient.GetAsync($"/api/timeentry?date={date1:yyyy-MM-dd}");
        var entries = await response.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();

        entries.Should().HaveCount(1);
        entries[0].Description.Should().Be("Today");
    }

    [Fact]
    public async Task Querying_time_entries_filters_by_month()
    {
        var taskId = await CreateActiveTaskAsync();
        var janEntry = new CreateTimeEntryRequest(new DateOnly(2026, 1, 15), 4, "January", taskId);
        var febEntry = new CreateTimeEntryRequest(new DateOnly(2026, 2, 10), 3, "February", taskId);

        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", janEntry);
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", febEntry);

        var response = await factory.HttpClient.GetAsync("/api/timeentry?month=1&year=2026");
        var entries = await response.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();

        entries.Should().HaveCount(1);
        entries[0].Description.Should().Be("January");
    }

    [Fact]
    public async Task Updating_time_entry_succeeds_and_persists_data()
    {
        var taskId = await CreateActiveTaskAsync();
        var date = DateOnly.FromDateTime(DateTime.Today);
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date, 3, "Original", taskId));

        var getResponse = await factory.HttpClient.GetAsync("/api/timeentry");
        var all = await getResponse.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();
        var entryId = all!.First().Id;

        var updateRequest = new UpdateTimeEntryRequest(date, 5, "Updated", taskId);
        var putResponse = await factory.HttpClient.PutAsJsonAsync($"/api/timeentry/{entryId}", updateRequest);

        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entry = await dbContext.TimeEntries.FindAsync(entryId);
        entry.Should().NotBeNull();
        entry!.Hours.Should().Be(5);
        entry.Description.Should().Be("Updated");
    }

    [Fact]
    public async Task Updating_time_entry_fails_when_entry_not_found()
    {
        var taskId = await CreateActiveTaskAsync();
        var request = new UpdateTimeEntryRequest(DateOnly.FromDateTime(DateTime.Today), 3, "Test", taskId);
        var response = await factory.HttpClient.PutAsJsonAsync($"/api/timeentry/{Guid.NewGuid()}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Updating_time_entry_fails_when_task_is_inactive_and_task_changes()
    {
        var projectId = await CreateProjectAsync();
        var taskId = await CreateTaskAsync(projectId, "WillBeInactive", true);
        var otherTaskId = await CreateTaskAsync(projectId, "Other", true);

        var date = DateOnly.FromDateTime(DateTime.Today);
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date, 3, "Test", taskId));

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var task = await dbContext.ProjectTasks.FindAsync(taskId);
            task!.IsActive = false;
            await dbContext.SaveChangesAsync();
        }

        var getResponse = await factory.HttpClient.GetAsync("/api/timeentry");
        var all = await getResponse.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();
        var entryId = all!.First().Id;

        var updateRequest = new UpdateTimeEntryRequest(date, 3, "Try change task", otherTaskId);
        var putResponse = await factory.HttpClient.PutAsJsonAsync($"/api/timeentry/{entryId}", updateRequest);

        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Deleting_existing_time_entry_returns_no_content()
    {
        var taskId = await CreateActiveTaskAsync();
        var date = DateOnly.FromDateTime(DateTime.Today);
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date, 3, "ToDelete", taskId));

        var getResponse = await factory.HttpClient.GetAsync("/api/timeentry");
        var all = await getResponse.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();
        var entryId = all!.First().Id;

        var deleteResponse = await factory.HttpClient.DeleteAsync($"/api/timeentry/{entryId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Deleting_non_existent_time_entry_returns_no_content()
    {
        var response = await factory.HttpClient.DeleteAsync($"/api/timeentry/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Deleting_time_entry_removes_it_from_the_list()
    {
        var taskId = await CreateActiveTaskAsync();
        var date = DateOnly.FromDateTime(DateTime.Today);
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date, 3, "Keep", taskId));
        await factory.HttpClient.PostAsJsonAsync("/api/timeentry", new CreateTimeEntryRequest(date, 2, "Remove", taskId));

        var getResponse = await factory.HttpClient.GetAsync("/api/timeentry");
        var all = await getResponse.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();
        var removeId = all!.First(e => e.Description == "Remove").Id;

        await factory.HttpClient.DeleteAsync($"/api/timeentry/{removeId}");

        var finalResponse = await factory.HttpClient.GetAsync("/api/timeentry");
        var final = await finalResponse.Content.ReadFromJsonAsync<IReadOnlyList<TimeEntryResponse>>();
        final.Should().HaveCount(1);
        final.Should().Contain(e => e.Description == "Keep");
        final.Should().NotContain(e => e.Description == "Remove");
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
