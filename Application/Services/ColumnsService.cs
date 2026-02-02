using Domain;
using Domain.Exceptions;

namespace Application.Services;

public class ColumnsService
{
    private readonly IColumnsRepository _columnsRepository;
    private readonly IBoardsRepository _boardsRepository;
    private readonly IBoardUsersRepository _boardUsersRepository;

    public ColumnsService(IColumnsRepository columnsRepository, 
        IBoardsRepository boardsRepository,
        IBoardUsersRepository boardUsersRepository)
    {
        _columnsRepository = columnsRepository;
        _boardsRepository = boardsRepository;
        _boardUsersRepository = boardUsersRepository;
    }

    public async Task<Column> GetByIdAsync(Guid columnId, Guid boardId) => await _columnsRepository.GetOneByBoardIdAsync(columnId, boardId) ?? throw new NullReferenceException("Column not found");

    public async Task<Column> AddAsync(string title, Guid boardId, Guid userId)
    {
        var board = await _boardsRepository.GetByIdAsync(boardId) ?? throw new NotFoundException("Board not found");
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) 
            ?? throw new ForbiddenException("You are not a user of the board");
        if ((int)boardUser.Role > 1)
            throw new ForbiddenException("You don't have enough rights");
        
        int order = board.Columns.Count;

        Column newColumn = new Column
        {
            Title = title,
            BoardId = boardId,
            Order = order
        };
        
        await _columnsRepository.AddAsync(newColumn);
        return newColumn;
    }

    public async Task<Column> UpdateAsync(Guid columnId, Guid userId, Guid boardId, string title, int newOrder)
    {
        if (await _boardsRepository.GetByIdAsync(boardId) == null)
            throw new NotFoundException("Board not found");
        var column = await _columnsRepository.GetOneByBoardIdAsync(columnId, boardId) ?? throw new NullReferenceException("Column not found");
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) 
                        ?? throw new ForbiddenException("You are not a user of the board");
        if ((int)boardUser.Role > 1)
            throw new ForbiddenException("You don't have enough rights");
        
        column.Title = title;
            
        List<Column> columns = (await _columnsRepository.GetAllByBoardIdAsync(column.BoardId, userId))
            .OrderBy(c => c.Order)
            .ToList();

        var oldIndex = columns.FindIndex(c => c.Id == columnId);
        if (column.Order == newOrder)
        {
            await _columnsRepository.UpdateAsync(column);
            return column;
        } 
        
        
        columns.RemoveAt(oldIndex);
        columns.Insert(newOrder, column);
        
        for (int i = 0; i < columns.Count; i++)
            columns[i].Order = i;

        await _columnsRepository.SaveChangesAsync();
        return column;
    }

    public async Task DeleteAsync(Guid columnId, Guid userId, Guid boardId)
    {
        if (await _boardsRepository.GetByIdAsync(boardId) == null) throw new NotFoundException("Board not found");
        var column = await _columnsRepository.GetOneByBoardIdAsync(columnId, boardId) ?? throw new NotFoundException("Column not found");
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) 
                        ?? throw new ForbiddenException("You are not a user of the board");
        if ((int)boardUser.Role > 1)
            throw new ForbiddenException("You don't have enough rights");
        
        List<Column> columns = (await _columnsRepository.GetAllByBoardIdAsync(column.BoardId, userId))
            .OrderBy(c => c.Order)
            .ToList();
        
        var deletedIndex = columns.FindIndex(c => c.Id == columnId);
        
        columns.RemoveAt(deletedIndex);
        
        for (int i = 0; i < columns.Count; i++)
            columns[i].Order = i;
        
        await _columnsRepository.DeleteAsync(columnId, userId);
        await _columnsRepository.SaveChangesAsync();
    }
}