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

    public async Task<Column> AddAsync(string name, Guid boardId, Guid userId)
    {
        var board = await _boardsRepository.GetOneByUserIdAsync(boardId, userId);
        int order = board.Columns.Count;

        Column newColumn = new Column
        {
            Name = name,
            BoardId = boardId,
            Order = order
        };
        
        await _columnsRepository.AddAsync(newColumn);
        return newColumn;
    }
}