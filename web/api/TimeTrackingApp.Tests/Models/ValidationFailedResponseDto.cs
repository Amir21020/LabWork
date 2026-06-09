namespace TimeTrackingApp.Tests.Models;

public sealed record ValidationErrorDto(string Field, string Message);

public sealed record ValidationFailedResponseDto(string Status, List<ValidationErrorDto> Errors);