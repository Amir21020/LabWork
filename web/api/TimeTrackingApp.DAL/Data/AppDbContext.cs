using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.DAL.Entities;

namespace TimeTrackingApp.DAL.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<ProjectTaskEntity> ProjectTasks { get; set; }
    public DbSet<TimeEntryEntity> TimeEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectTaskEntity>()
            .HasOne(t => t.Project)
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeEntryEntity>()
            .HasOne(t => t.Task)
            .WithMany()
            .HasForeignKey(t => t.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}