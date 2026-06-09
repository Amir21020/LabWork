using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.Tests.IntegrationTests;

[Collection("ProjectsCollection")]
public class ProjectControllerTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public async Task DisposeAsync()
        => await factory.ResetDatabaseAsync();

    public Task InitializeAsync()
       => Task.CompletedTask;

    [Fact]
    public async Task Querying_projects_returns_empty_list_when_there_are_none()
    {
        var response = await factory.HttpClient.GetAsync("/api/project");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetProjectsResponse>>();
        projects.Should().BeEmpty();
    }

    [Fact]
    public async Task Querying_projects_returns_all_existing_projects()
    {
        await CreateProjectAsync("Alpha", "ALP", isActive: true);
        await CreateProjectAsync("Beta", "BET", isActive: false);

        var response = await factory.HttpClient.GetAsync("/api/project");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetProjectsResponse>>();

        projects.Should().HaveCount(2);
        projects.Should().ContainSingle(p => p.Name == "Alpha" && p.Code == "ALP" && p.IsActive == true);
        projects.Should().ContainSingle(p => p.Name == "Beta" && p.Code == "BET" && p.IsActive == false);
    }

    private async Task CreateProjectAsync(string name, string code, bool isActive)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var project = new ProjectEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            IsActive = isActive
        };

        await dbContext.AddAsync(project);
        await dbContext.SaveChangesAsync();
    }
}
