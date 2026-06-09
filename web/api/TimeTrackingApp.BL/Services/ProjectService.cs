using Microsoft.Extensions.Logging;
using TimeTrackingApp.BL.DTOs;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;

namespace TimeTrackingApp.BL.Services;

public sealed class ProjectService(
    IBaseRepository<ProjectEntity> projectRepository,
    ILogger<ProjectService> logger) : IProjectService
{
    public async Task CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        logger.LogInformation("Creating project with Name={Name}, Code={Code}", request.Name, request.Code);

        var project = ProjectEntity.Create(request.Name, request.Code, request.IsActive);
        await projectRepository.AddAsync(project, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting project with Id={Id}", id);

        var project = await projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            logger.LogWarning("Project with Id={Id} not found for deletion", id);
            return;
        }

        await projectRepository.DeleteAsync(project, ct);

        logger.LogInformation("Project with Id={Id} deleted", id);
    }

    public async Task<IReadOnlyList<GetProjectsResponse>> GetAllAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Fetching all projects");

        var projects = await projectRepository.GetAllAsync(ct);

        logger.LogInformation("Successfully retrieved {Count} projects", projects.Count);

        return projects
            .Select(entity => new GetProjectsResponse(
                entity.Id,
                entity.Name,
                entity.Code,
                entity.IsActive
            ))
            .ToList();
    }
}
