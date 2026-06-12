namespace TimeTrackingApp.BL.DTOs;

public sealed record UpdateTaskRequest(string Name, bool IsActive);