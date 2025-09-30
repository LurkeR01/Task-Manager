using Domain;

namespace Application.Services;

public class TasksService
{
    private readonly ITasksRepository _tasksRepository;
    
    public TasksService(ITasksRepository tasksRepository)
    {
        _tasksRepository = tasksRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync() => await _tasksRepository.GetAllAsync();
    
    public async Task<TaskItem> GetByIdAsync(Guid id) => await _tasksRepository.GetByIdAsync(id);

    public async Task AddAsync(string title, string description, DateTime dueDate)
    {
        TaskItem taskItem = new TaskItem
        {
            Title = title,
            Description = description,
            DueDate = dueDate
        };
        
        await _tasksRepository.AddAsync(taskItem);
    }

    public async Task UpdateAsync(Guid id, string title, string description, DateTime dueDate, bool isDone)
    {
        var task = await _tasksRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        
        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.IsDone = isDone;
        
        await _tasksRepository.UpdateAsync(task);
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _tasksRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        
        await _tasksRepository.DeleteAsync(task);
    }
}