namespace TimeTrackingApp.BL.DTOs;

public sealed record CreateTaskRequest(string Name, Guid ProjectId, bool IsActive);