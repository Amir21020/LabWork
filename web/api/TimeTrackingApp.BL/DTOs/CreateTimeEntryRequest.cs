namespace TimeTrackingApp.BL.DTOs;

public sealed record CreateTimeEntryRequest(DateOnly Date, int Hours, string Description, Guid TaskId);