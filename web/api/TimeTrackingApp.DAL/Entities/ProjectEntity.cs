namespace TimeTrackingApp.DAL.Entities;

public sealed class ProjectEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
