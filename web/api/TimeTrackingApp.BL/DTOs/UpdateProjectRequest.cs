namespace TimeTrackingApp.BL.DTOs;

public sealed record UpdateProjectRequest(string Name, string Code, bool IsActive);