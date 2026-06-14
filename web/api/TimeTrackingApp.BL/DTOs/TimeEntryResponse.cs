namespace TimeTrackingApp.BL.DTOs;

public sealed record TimeEntryResponse(
    Guid Id,
    DateOnly Date,
    int Hours,
    string Description,
    Guid TaskId,
    string TaskName
);