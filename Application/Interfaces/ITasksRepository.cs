using Domain;

namespace Application;

public interface ITasksRepository
{
    Task<TaskItem> GetOneByColumnIdAsync(Guid taskItemId, Guid columnId);
    Task AddAsync(TaskItem newTask);
    Task UpdateAsync(TaskItem updatedTask);
    Task DeleteAsync(TaskItem taskItem, Guid ownerId);
}