using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<ProjectTaskEntity> ProjectTasks { get; set; }
    public DbSet<TimeEntryEntity> TimeEntries { get; set; }
}