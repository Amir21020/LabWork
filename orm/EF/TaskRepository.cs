using Orm.Data;
using Orm.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Orm.EF
{
    public sealed class TaskRepository
    {
        private readonly string _connectionString;
        public TaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task AddAsync(TaskEntity task)
        {
            using (var taskDbContext = new TaskDbContext())
            {
                taskDbContext.Tasks.Add(task);
                await taskDbContext.SaveChangesAsync();
            }
        }
        public async Task<List<TaskEntity>> GetAllAsync()
        {
            using (var taskDbContext = new TaskDbContext())
            {
                return await taskDbContext.Tasks.ToListAsync();
            }
        }
        public async Task<TaskEntity> GetByIdAsync(Guid id)
        {
            using (var taskDbContext = new TaskDbContext())
            {
                return await taskDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            }
        }
        public async Task UpdateAsync(TaskEntity task)
        {
            using (var taskDbContext = new TaskDbContext())
            {
                var existing = await taskDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
                if (existing != null)
                {
                    existing.Name = task.Name;
                    existing.Description = task.Description;
                    await taskDbContext.SaveChangesAsync();
                }
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            using (var taskDbContext = new TaskDbContext())
            {
                var task = await taskDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
                if (task != null)
                {
                    taskDbContext.Tasks.Remove(task);
                    await taskDbContext.SaveChangesAsync();
                }
            }
        }
    }
}
