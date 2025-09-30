using Domain;

namespace Application;

public interface ITasksRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem> GetByIdAsync(Guid id);
    Task AddAsync(TaskItem newTask);
    Task UpdateAsync(TaskItem updatedTask);
    Task DeleteAsync(TaskItem deletedTask);
}