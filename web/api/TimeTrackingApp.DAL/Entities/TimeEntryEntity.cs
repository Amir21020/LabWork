namespace TimeTrackingApp.DAL.Entities;

public sealed class TimeEntryEntity : BaseEntity
{
    public DateOnly Date { get; set; }
    public int Hours { get; set; }
    public string? Description { get; set; }
    public Guid TaskId { get; set; }
    public ProjectTaskEntity Task { get; set; }

    private TimeEntryEntity()
    {
        Date = new DateOnly();
        Description = string.Empty;
    }

    private TimeEntryEntity(DateOnly date, Guid taskId, int hours, string description)
    {
        Date = date;
        Hours = hours;
        Description = description;
        TaskId = taskId;
    }

    public static TimeEntryEntity Create(DateOnly date, Guid taskId, int hours, string description)
        => new TimeEntryEntity(date, taskId, hours, description);
}
