using Domain;
using Domain.Exceptions;

namespace Application.Services;

public class TasksService
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IColumnsRepository _columnRepository;
    private readonly IBoardUsersRepository _boardUsersRepository;
    
    public TasksService(ITasksRepository tasksRepository, 
        IColumnsRepository columnRepository,
        IBoardUsersRepository boardUsersRepository)
    {
        _tasksRepository = tasksRepository;
        _columnRepository = columnRepository;
        _boardUsersRepository = boardUsersRepository;
    }

    public async Task<TaskItem> GetByIdAsync(Guid taskItemId, Guid columnId)
    {
        if (await _columnRepository.GetByIdAsync(columnId) == null)
            throw new NotFoundException("Column not found");
        var taskItem = await _tasksRepository.GetByIdAsync(taskItemId) 
            ?? throw new NotFoundException("Task not found");
        
        return taskItem;
    }

    public async Task<TaskItem> AddAsync(string title, 
        string description, 
        DateTime? dueDate, 
        Guid columnId, 
        Guid userId)
    {
        if (await _columnRepository.GetByIdAsync(columnId) == null)
            throw new NotFoundException("Column not found");
        
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) 
                        ?? throw new ForbiddenException("You are not a user of the board");
        if ((int)boardUser.Role > 2)
            throw new ForbiddenException("You don't have enough rights");
        
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
        Guid userId,
        Guid curretnColumnId, 
        string title, 
        string description, 
        DateTime dueDate,
        Guid targetColumnId)
    {
        
        if (await _columnRepository.GetByIdAsync(curretnColumnId) == null)
            throw new NotFoundException("Current column not found");
        var task = await _tasksRepository.GetByIdAsync(taskItemId) 
                   ?? throw new NotFoundException("Task not found");
        if (await _columnRepository.GetByIdAsync(targetColumnId) == null)
            throw new NotFoundException("Target column not found");
        
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) 
                        ?? throw new ForbiddenException("You are not a user of the board");
        if ((int)boardUser.Role > 2)
            throw new ForbiddenException("You don't have enough rights");
        
        
        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.ColumnId = targetColumnId;
        
        await _tasksRepository.UpdateAsync(task);
        return task;
    }

    public async Task DeleteAsync(Guid taskItemId, Guid columnId, Guid userId)
    {
        if (await _columnRepository.GetByIdAsync(columnId) == null)
            throw new NotFoundException("Column not found");
        var task = await _tasksRepository.GetByIdAsync(taskItemId) 
                   ?? throw new NotFoundException("Task not found");
        
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) 
                        ?? throw new ForbiddenException("You are not a user of the board");
        if ((int)boardUser.Role > 2)
            throw new ForbiddenException("You don't have enough rights");
        await _tasksRepository.DeleteAsync(task, userId);
    }
}