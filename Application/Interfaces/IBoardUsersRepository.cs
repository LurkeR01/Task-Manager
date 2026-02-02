using Domain;

namespace Application;

public interface IBoardUsersRepository
{
    Task<BoardUser> GetByUserIdAsync(Guid userId);
    Task<BoardUser> GetByBoardIdAsync(Guid boardId);
    Task AddBoardUserAsync(BoardUser boardUser);
}