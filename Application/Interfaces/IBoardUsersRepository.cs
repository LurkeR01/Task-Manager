using Domain;

namespace Application;

public interface IBoardUsersRepository
{
    Task<BoardUser> GetByUserIdAsync(Guid userId);
    Task AddBoardUserAsync(BoardUser boardUser);
}