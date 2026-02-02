using System.Collections;
using System.Security.Authentication;
using Domain;
using Domain.Exceptions;

namespace Application.Services;

public class BoardsService
{
    private readonly IBoardsRepository _boardsRepository;
    private readonly ColumnsService _columnsService;
    private readonly IUnitOfWork _uof;
    private readonly IBoardUsersRepository _boardUsersRepository;

    public BoardsService(
        IBoardsRepository boardsRepository,
        ColumnsService columnsService,
        IUnitOfWork uof,
        IBoardUsersRepository boardUsersRepository
    )
    {
        _boardsRepository = boardsRepository;
        _columnsService = columnsService;
        _uof = uof;
        _boardUsersRepository = boardUsersRepository;
    }

    public async Task<IEnumerable<Board>> GetAllAsync(Guid userId)
    {
        var boards = await _boardsRepository.GetAllByUserIdAsync(userId);
        return boards;
    }

    public async Task<Board> GetByIdAsync(Guid boardId, Guid userId)
    {
        var board = await _boardsRepository.GetByIdAsync(boardId) 
            ?? throw new NotFoundException("Board not found");
        if (await _boardUsersRepository.GetByUserIdAsync(userId) == null)
            throw new ForbiddenException("You are not member of the board");

        return board;
    }

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
                new Column { BoardId = newBoardId, Title = "To do" },
                new Column { BoardId = newBoardId, Title = "In progress" },
                new Column { BoardId = newBoardId, Title = "Done" }
            };

            foreach (var column in columns)
            {
                await _columnsService.AddAsync(column.Title, column.BoardId, ownerId);
            }
            
            BoardUser newBoardUser = new BoardUser
            {
                BoardId = newBoardId,
                UserId = ownerId,
                Role = Roles.Owner
            };
            await _boardUsersRepository.AddBoardUserAsync(newBoardUser);

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
        var board = await _boardsRepository.GetOneByUserIdAsync(boardId, ownerId) ?? 
                    throw new NotFoundException("Board not found");

        board.Title = title;

        await _boardsRepository.UpdateAsync(board);
        return board;
    }

    public async Task DeleteAsync(Guid boardId, Guid ownerId)
    {
        if(await _boardsRepository.GetOneByUserIdAsync(boardId, ownerId) == null)
            throw new NotFoundException("Board not found");
        await _boardsRepository.DeleteAsync(boardId, ownerId);
    }
}