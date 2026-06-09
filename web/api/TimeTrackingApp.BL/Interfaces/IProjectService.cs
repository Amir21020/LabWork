using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<GetProjectsResponse>> GetAllAsync(CancellationToken ct = default);
}
