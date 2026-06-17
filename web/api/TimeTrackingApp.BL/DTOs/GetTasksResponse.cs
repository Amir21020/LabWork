namespace TimeTrackingApp.BL.DTOs;

public sealed record GetTasksResponse(Guid Id, string Name, Guid ProjectId, bool IsActive);