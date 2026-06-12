namespace TimeTrackingApp.BL.DTOs;

public sealed record GetTasksResponse(Guid Id, string Name, Guid ProjectId, string ProjectName, bool IsActive);