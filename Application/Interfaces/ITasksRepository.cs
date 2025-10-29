using Domain;

namespace Application;

public interface ITasksRepository
{
    Task<IEnumerable<TaskItem>> GetAllByUserIdAsync(Guid userId);
    Task<TaskItem> GetOneByUserIdAsync(Guid taskItemId, Guid userId);
    Task AddAsync(TaskItem newTask);
    Task UpdateAsync(TaskItem updatedTask);
    Task DeleteAsync(TaskItem deletedTask);
}