using System.Collections;
using Domain;

namespace Application.Services;

public class BoardsService
{
    private readonly IBoardsRepository _boardsRepository;
    private readonly ColumnsService _columnsService;
    private readonly IUnitOfWork _uof;

    public BoardsService(
        IBoardsRepository boardsRepository, 
        ColumnsService columnsService,
        IUnitOfWork uof
        )
    {
        _boardsRepository = boardsRepository;
        _columnsService = columnsService;
        _uof = uof;
    }

    public async Task<IEnumerable<Board>> GetAllAsync(Guid userId)
    {
        var boards  = await _boardsRepository.GetAllByUserIdAsync(userId);
        return boards;
    }
    
    public async Task<Board> GetByIdAsync(Guid boardId, Guid userId) => await _boardsRepository.GetOneByUserIdAsync(boardId, userId);

    public async Task<Board> AddAsync(string title, Guid ownerId)
    {
        await _uof.BeginTransactionAsync();

        try
        {
            Guid newBoardId = Guid.NewGuid();

            Board newBoard = new Board
            {
                Id = newBoardId,
                Title = title,
                OwnerId = ownerId
            };

            await _boardsRepository.AddAsync(newBoard);

            List<Column> columns = new List<Column>
            {
                new Column { BoardId = newBoardId, Name = "To do" },
                new Column { BoardId = newBoardId, Name = "In progress" },
                new Column { BoardId = newBoardId, Name = "Done" }
            };

            foreach (var column in columns)
            {
                await _columnsService.AddAsync(column.Name, column.BoardId, ownerId);
            }

            await _uof.CommitAsync();

            newBoard.Columns = columns;
            return newBoard;
        }
        catch
        {
            await _uof.RollbackAsync();
            throw;
        }
    }

    public async Task<Board> UpdateAsync(Guid boardId, Guid ownerId, string title)
    {
        var board = await _boardsRepository.GetOneByUserIdAsync(boardId, ownerId) ?? throw new NullReferenceException("Board not found");
        
        board.Title = title;
        
        await _boardsRepository.UpdateAsync(board);
        return board;
    }

    public async Task DeleteAsync(Guid boardId) =>  await _boardsRepository.DeleteAsync(boardId);
}