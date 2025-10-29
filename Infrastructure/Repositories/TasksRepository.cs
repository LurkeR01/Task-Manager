using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TasksRepository : ITasksRepository
{
    private readonly AppDbContext _dbContext;
    
    public TasksRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllByUserIdAsync(Guid userId)
    {
        return await _dbContext.TaskItems
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TaskItem> GetOneByUserIdAsync(Guid taskItemId, Guid userId)
    {
        return await _dbContext.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskItemId && t.UserId == userId);
    }

    public async Task AddAsync(TaskItem newTask) {
        await _dbContext.TaskItems.AddAsync(newTask);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem updatedTask)
    {
        await _dbContext.TaskItems.Where(t => t.Id == updatedTask.Id)
            .ExecuteUpdateAsync(t => t
                .SetProperty(s => s.Title, updatedTask.Title)
                .SetProperty(s => s.Description, updatedTask.Description)
                .SetProperty(s => s.DueDate, updatedTask.DueDate)
                .SetProperty(s => s.IsDone, updatedTask.IsDone));
    }

    public async Task DeleteAsync(TaskItem deletedTask)
    {
        await _dbContext.TaskItems
            .Where(t => t.Id == deletedTask.Id)
            .ExecuteDeleteAsync();
    }
}