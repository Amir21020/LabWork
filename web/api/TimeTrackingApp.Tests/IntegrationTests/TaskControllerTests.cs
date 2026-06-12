using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;

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
        await CreateTasksAsync("Alpha", isActive: true);
        await CreateTasksAsync("Beta", isActive: false);

        var response = await factory.HttpClient.GetAsync("/api/task");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetTasksResponse>>();

        projects.Should().HaveCount(2);
        projects.Should().ContainSingle(p => p.Name == "Alpha" && p.IsActive == true);
        projects.Should().ContainSingle(p => p.Name == "Beta" && p.IsActive == false);

    }

    private async Task CreateTasksAsync(string name, bool isActive)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tasks = ProjectTaskEntity.Create(name, isActive);

        await dbContext.AddAsync(tasks);
        await dbContext.SaveChangesAsync();
    }
}
