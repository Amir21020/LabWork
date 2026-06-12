using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Services;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Repositories;
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

    [Fact]
    public async Task Deleting_existing_task_returns_no_content()
    {
        var taskId = await CreateTaskAsync("ToDelete", true);

        var response = await factory.HttpClient.DeleteAsync($"/api/task/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Deleting_non_existent_task_returns_no_content()
    {
        var response = await factory.HttpClient.DeleteAsync($"/api/task/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Deleting_task_removes_it_from_the_list()
    {
        await CreateTaskAsync("Keep", true);
        var taskId = await CreateTaskAsync("Remove", true);

        var deleteResponse = await factory.HttpClient.DeleteAsync($"/api/task/{taskId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await factory.HttpClient.GetAsync("/api/task");
        var tasks = await getResponse.Content.ReadFromJsonAsync<IReadOnlyList<GetTasksResponse>>();

        tasks.Should().HaveCount(1);
        tasks.Should().ContainSingle(t => t.Name == "Keep");
        tasks.Should().NotContain(t => t.Name == "Remove");
    }

    [Fact]
    public async Task Updating_existing_task_returns_no_content()
    {
        var taskId = await CreateTaskAsync("OldName", true);

        var updateRequest = new UpdateTaskRequest("NewName", false);
        var response = await factory.HttpClient.PutAsJsonAsync($"/api/task/{taskId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Updating_task_changes_its_data()
    {
        var taskId = await CreateTaskAsync("Original", true);

        var updateRequest = new UpdateTaskRequest("Updated", false);
        await factory.HttpClient.PutAsJsonAsync($"/api/task/{taskId}", updateRequest);

        var getResponse = await factory.HttpClient.GetAsync("/api/task");
        var tasks = await getResponse.Content.ReadFromJsonAsync<IReadOnlyList<GetTasksResponse>>();

        tasks.Should().ContainSingle(t =>
            t.Id == taskId &&
            t.Name == "Updated" &&
            t.IsActive == false);
    }

    [Fact]
    public async Task Updating_task_with_empty_name_returns_bad_request()
    {
        var taskId = await CreateTaskAsync("Valid", true);

        var updateRequest = new UpdateTaskRequest("", true);
        var response = await factory.HttpClient.PutAsJsonAsync($"/api/task/{taskId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationFailedResponseDto>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Status.Should().Be("ValidationFailed");
        errorResponse.Errors.Should().Contain(e =>
            e.Field == "Name" && e.Message == "Название задачи обязательно для заполнения.");
    }

    [Fact]
    public async Task Updating_non_existent_task_returns_no_content()
    {
        var updateRequest = new UpdateTaskRequest("Ghost", true);
        var response = await factory.HttpClient.PutAsJsonAsync($"/api/task/{Guid.NewGuid()}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
