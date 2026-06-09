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

    [Fact]
    public async Task Creating_project_returns_bad_request_when_name_is_empty()
    {
        var request = new CreateProjectRequest(
            Name: string.Empty,
            Code: "PRJ-101",
            IsActive: true
        );

        var response = await factory.HttpClient.PostAsJsonAsync("/api/project", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationFailedResponseDto>();

        errorResponse.Should().NotBeNull();
        errorResponse!.Status.Should().Be("ValidationFailed");

        errorResponse.Errors.Should().ContainSingle(e =>
            e.Field == "Name" &&
            e.Message == "Название проекта обязательно для заполнения.");
    }

    [Fact]
    public async Task Creating_project_fails_when_multiple_required_fields_are_empty()
    {
        var request = new CreateProjectRequest(
            Name: string.Empty,
            Code: string.Empty,
            IsActive: true
        );

        var response = await factory.HttpClient.PostAsJsonAsync("/api/project", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationFailedResponseDto>();

        errorResponse.Should().NotBeNull();
        errorResponse!.Status.Should().Be("ValidationFailed");

        errorResponse.Errors.Should().HaveCount(3);

        errorResponse.Errors.Should().Contain(e =>
            e.Field == "Name" &&
            e.Message == "Название проекта обязательно для заполнения.");

        errorResponse.Errors.Should().Contain(e =>
            e.Field == "Code" &&
            e.Message == "Код проекта обязателен для заполнения.");

        errorResponse.Errors.Should().Contain(e =>
            e.Field == "Code" &&
            e.Message == "Код проекта может содержать только заглавные латинские буквы, цифры, дефис и нижнее подчеркивание (без пробелов).");
    }

    [Fact]
    public async Task Creating_project_succeeds_when_data_is_valid()
    {
        var request = new CreateProjectRequest(
            Name: "Мой тестовый проект",
            Code: "PROJ-123",
            IsActive: true
        );

        var response = await factory.HttpClient.PostAsJsonAsync("/api/project", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task CreateProjectAsync(string name, string code, bool isActive)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var project = ProjectEntity.Create(name, code, isActive);

        await dbContext.AddAsync(project);
        await dbContext.SaveChangesAsync();
    }
}
