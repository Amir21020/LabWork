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
