namespace TimeTrackingApp.BL.DTOs;

public sealed record GetProjectsResponse(Guid ProjectId, string Name, string Code, bool IsActive);