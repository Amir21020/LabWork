namespace TimeTrackingApp.BL.DTOs;

public sealed record GetTimeEntriesRequest(DateOnly? Date, int? Month, int? Year);