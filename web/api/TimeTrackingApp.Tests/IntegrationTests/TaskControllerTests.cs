using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.Tests.Models;

namespace TimeTrackingApp.Tests.IntegrationTests;

[Collection("ProjectsCollection")]
public sealed class TaskControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task DisposeAsync()
        => factory.ResetDatabaseAsync();

    public Task InitializeAsync()
        => Task.CompletedTask;

    [Fact]
    public async Task Querying_tasks_returns_empty_list_when_there_are_none()
    {
        var response = await factory.HttpClient.GetAsync("/api/task");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetTasksResponse>>();

        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task Querying_tasks_returns_all_existing_tasks()
    {
        await CreateTaskAsync("Alpha", true);
        await CreateTaskAsync("Beta", false);

        var response = await factory.HttpClient.GetAsync("/api/task");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetTasksResponse>>();

        tasks.Should().HaveCount(2);
        tasks.Should().ContainSingle(t => t.Name == "Alpha" && t.IsActive == true);
        tasks.Should().ContainSingle(t => t.Name == "Beta" && t.IsActive == false);
    }

    [Fact]
    public async Task Creating_task_succeeds_when_data_is_valid()
    {
        var projectId = await CreateProjectAndReturnIdAsync();

        var request = new CreateTaskRequest("Test Task", projectId, true);
        var response = await factory.HttpClient.PostAsJsonAsync("/api/task", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Creating_task_returns_bad_request_when_name_is_empty()
    {
        var projectId = await CreateProjectAndReturnIdAsync();

        var request = new CreateTaskRequest("", projectId, true);
        var response = await factory.HttpClient.PostAsJsonAsync("/api/task", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationFailedResponseDto>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Status.Should().Be("ValidationFailed");
        errorResponse.Errors.Should().Contain(e =>
            e.Field == "Name" &&
            e.Message == "Название задачи обязательно для заполнения.");
    }

    [Fact]
    public async Task Creating_task_returns_bad_request_when_project_not_found()
    {
        var request = new CreateTaskRequest("Orphan Task", Guid.NewGuid(), true);
        var response = await factory.HttpClient.PostAsJsonAsync("/api/task", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateProjectAndReturnIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var project = ProjectEntity.Create("TestProject", "TST", true);
        await dbContext.AddAsync(project);
        await dbContext.SaveChangesAsync();

        return project.Id;
    }

    private async Task<Guid> CreateTaskAsync(string name, bool isActive)
    {
        var projectId = await CreateProjectAndReturnIdAsync();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var task = ProjectTaskEntity.Create(name, projectId, isActive);
        await dbContext.AddAsync(task);
        await dbContext.SaveChangesAsync();

        return task.Id;
    }
}
