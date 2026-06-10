namespace TimeTrackingApp.DAL.Entities;

public sealed class ProjectEntity : BaseEntity
{
    public string Name { get; set; } 
    public string Code { get; set; } 
    public bool IsActive { get; set; }

    private ProjectEntity()
    {
        Name = string.Empty;
        Code = string.Empty;
    }

    private ProjectEntity(string name, string code, bool isActive)
    {
        Name = name;
        Code = code;
        IsActive = isActive;
    }

    public static ProjectEntity Create(string name, string code, bool isActive = true)
        => new ProjectEntity(name, code, isActive);
}
