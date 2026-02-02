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

    public async Task<TaskItem> GetByIdAsync(Guid taskItemId)
    {
        return await _dbContext.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskItemId);
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
                .SetProperty(s => s.ColumnId, updatedTask.ColumnId));
    }

    public async Task DeleteAsync(TaskItem taskItem, Guid ownerId)
    {
        await _dbContext.TaskItems
            .Where(t => t.Id == taskItem.Id)
            .ExecuteDeleteAsync();
    }
}