using Orm.Entities;
using System.Data.Entity;

namespace Orm.Data
{
    public sealed class TaskDbContext : DbContext
    {
        public DbSet<TaskEntity> Tasks { get; set; }
        
        public TaskDbContext() : base("name=DefaultConnection")
        {
        
        }
    }
}
