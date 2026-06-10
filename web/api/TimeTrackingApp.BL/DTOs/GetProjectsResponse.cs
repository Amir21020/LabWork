namespace TimeTrackingApp.BL.DTOs;

public sealed record GetProjectsResponse(Guid Id, string Name, string Code, bool IsActive);