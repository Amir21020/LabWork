using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orm.Entities
{
    [Table("Tasks")]
    public sealed class TaskEntity
    {
        public TaskEntity() { }
        public TaskEntity(string name, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
