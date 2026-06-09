namespace TimeTrackingApp.DAL.Entities;

public sealed class ProjectEntity : BaseEntity
{
    public string Name { get; private set; } 
    public string Code { get; private set; } 
    public bool IsActive { get; private set; }

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
