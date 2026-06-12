namespace TimeTrackingApp.DAL.Entities;

public sealed class ProjectTaskEntity : BaseEntity
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectEntity Project { get; set;  }

    private ProjectTaskEntity()
    {
        Name = string.Empty;
    }

    private ProjectTaskEntity(string name, Guid projectId, bool isActive)
    {
        Name = name;
        IsActive = isActive;
        ProjectId = projectId;
    }

    public static ProjectTaskEntity Create(string name, Guid projectId, bool isActive = true)
        => new ProjectTaskEntity(name, projectId, isActive);
}
