using Domain;

namespace Application.Services;

public class ColumnsService
{
    private readonly IColumnsRepository _columnsRepository;
    private readonly IBoardsRepository _boardsRepository;

    public ColumnsService(IColumnsRepository columnsRepository, IBoardsRepository boardsRepository)
    {
        _columnsRepository = columnsRepository;
        _boardsRepository = boardsRepository;
    }

    public async Task<Column> GetByIdAsync(Guid columnId, Guid boardId) => await _columnsRepository.GetOneByBoardIdAsync(columnId, boardId) ?? throw new NullReferenceException("Column not found");

    public async Task<Column> AddAsync(string title, Guid boardId, Guid userId)
    {
        var board = await _boardsRepository.GetOneByUserIdAsync(boardId, userId) ?? throw new Exception("Board not found");
        if (board.OwnerId != userId)
            throw new UnauthorizedAccessException("You are not authorized to access this board");
        
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

    public async Task<Column> UpdateAsync(Guid columnId, Guid ownerId, string title, int newOrder)
    {
        var column = await _columnsRepository.GetOneByBoardIdAsync(columnId, ownerId) ?? throw new NullReferenceException("Column not found");
        
        column.Title = title;
            
        List<Column> columns = (await _columnsRepository.GetAllByBoardIdAsync(column.BoardId, ownerId))
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

    public async Task DeleteAsync(Guid columnId, Guid ownerId)
    {
        var column = await _columnsRepository.GetOneByBoardIdAsync(columnId, ownerId) ?? throw new NullReferenceException("Column not found");
        
        List<Column> columns = (await _columnsRepository.GetAllByBoardIdAsync(column.BoardId, ownerId))
            .OrderBy(c => c.Order)
            .ToList();
        
        var deletedIndex = columns.FindIndex(c => c.Id == columnId);
        
        columns.RemoveAt(deletedIndex);
        
        for (int i = 0; i < columns.Count; i++)
            columns[i].Order = i;
        
        await _columnsRepository.DeleteAsync(columnId, ownerId);
        await _columnsRepository.SaveChangesAsync();
    }
}