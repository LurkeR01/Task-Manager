using Domain;

namespace Application;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem> GetTaskByIdAsync(int id);
    Task AddTaskAsync(TaskItem newTask);
    Task UpdateTaskAsync(TaskItem updatedTask);
    Task DeleteTaskAsync(TaskItem deletedTask);
}