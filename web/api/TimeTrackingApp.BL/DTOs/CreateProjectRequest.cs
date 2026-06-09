namespace TimeTrackingApp.BL.DTOs;

public sealed record CreateProjectRequest(string Name, string Code, bool IsActive = true);