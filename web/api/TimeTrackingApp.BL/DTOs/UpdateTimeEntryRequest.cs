namespace TimeTrackingApp.BL.DTOs;

public sealed record UpdateTimeEntryRequest(DateOnly Date, int Hours, string Description, Guid TaskId);
