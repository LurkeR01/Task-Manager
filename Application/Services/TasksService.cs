using Domain;

namespace Application.Services;

public class TasksService
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IUsersRepository _usersRepository;
    
    public TasksService(ITasksRepository tasksRepository, IUsersRepository usersRepository)
    {
        _tasksRepository = tasksRepository;
        _usersRepository = usersRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid userId) => await _tasksRepository.GetAllByUserIdAsync(userId);
    
    public async Task<TaskItem> GetByIdAsync(Guid taskId, Guid userId) => await _tasksRepository.GetOneByUserIdAsync(taskId, userId);

    public async Task AddAsync(string title, string description, DateTime dueDate, Guid userId)
    {
        TaskItem taskItem = new TaskItem
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            UserId = userId
        };
        
        await _tasksRepository.AddAsync(taskItem);
    }

    public async Task UpdateAsync(Guid taskId, Guid userId, string title, string description, DateTime dueDate, bool isDone)
    {
        var task = await _tasksRepository.GetOneByUserIdAsync(taskId, userId) ?? throw new Exception("Task not found");
        
        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.IsDone = isDone;
        
        await _tasksRepository.UpdateAsync(task);
    }

    public async Task DeleteAsync(Guid taskId, Guid userId) => await _tasksRepository.DeleteAsync(taskId);
}