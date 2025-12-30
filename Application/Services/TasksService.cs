using Domain;

namespace Application.Services;

public class TasksService
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IColumnsRepository _columnRepositroy;
    
    public TasksService(ITasksRepository tasksRepository, 
        IUsersRepository usersRepository,
        IColumnsRepository columnRepositroy)
    {
        _tasksRepository = tasksRepository;
        _usersRepository = usersRepository;
        _columnRepositroy = columnRepositroy;
    }
    
    public async Task<TaskItem> GetByIdAsync(Guid taskItemId, Guid columnId) => await _tasksRepository.GetOneByColumnIdAsync(taskItemId, columnId) ?? throw new Exception("Task not found");

    public async Task<TaskItem> AddAsync(string title, 
        string description, 
        DateTime? dueDate, 
        Guid columnId, 
        Guid ownerId)
    {
        var column = await _columnRepositroy.GetByIdWithBoardForUserAsync(columnId, ownerId);
        if (column == null)
            throw new UnauthorizedAccessException("You are not authorized to access this board");
        
        TaskItem taskItem = new TaskItem
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            ColumnId = columnId
        };
        
        await _tasksRepository.AddAsync(taskItem);
        return taskItem;
    }

    public async Task<TaskItem> UpdateAsync(Guid taskItemId, 
        Guid ownerId,
        Guid columnId, 
        string title, 
        string description, 
        DateTime dueDate,
        Guid updatedColumnId)
    {
        var column = await _columnRepositroy.GetByIdWithBoardForUserAsync(columnId, ownerId);
        if (column == null)
            throw new UnauthorizedAccessException("You are not authorized to access this board");
        
        var task = await _tasksRepository.GetOneByColumnIdAsync(taskItemId, columnId) ?? throw new Exception("Task not found");
        
        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.ColumnId = updatedColumnId;
        
        await _tasksRepository.UpdateAsync(task);
        return task;
    }

    public async Task DeleteAsync(Guid taskItemId, Guid columnId, Guid ownerId)
    {
        var task = await _tasksRepository.GetOneByColumnIdAsync(taskItemId, columnId) ?? throw new Exception("Task not found");
        var column = await _columnRepositroy.GetByIdWithBoardForUserAsync(columnId, ownerId);
        if (column == null)
            throw new UnauthorizedAccessException("You are not authorized to access this board");
        await _tasksRepository.DeleteAsync(task, ownerId);
    }
}