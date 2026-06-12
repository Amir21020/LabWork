namespace TimeTrackingApp.DAL.Entities;

public sealed class ProjectTaskEntity : BaseEntity
{
    public string Name { get; set; }
    public ProjectEntity Project { get; set;  }
    public bool IsActive { get; set; }

    private ProjectTaskEntity()
    {
        Name = string.Empty;
    }

    private ProjectTaskEntity(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
    }

    public static ProjectTaskEntity Create(string name, bool isActive = true)
        => new ProjectTaskEntity(name, isActive);
}
