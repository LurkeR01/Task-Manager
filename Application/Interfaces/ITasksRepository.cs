using Domain;

namespace Application;

public interface ITasksRepository
{
    Task<TaskItem> GetByIdAsync(Guid taskItemId);
    Task AddAsync(TaskItem newTask);
    Task UpdateAsync(TaskItem updatedTask);
    Task DeleteAsync(TaskItem taskItem, Guid ownerId);
}