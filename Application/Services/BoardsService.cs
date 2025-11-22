using System.Collections;
using Domain;

namespace Application.Services;

public class BoardsService
{
    private readonly IBoardsRepository _boardsRepository;

    public BoardsService(IBoardsRepository boardsRepository)
    {
        _boardsRepository = boardsRepository;
    }
    
    public async Task<IEnumerable<Board>> GetAllAsync(Guid userId) => await _boardsRepository.GetAllByUserIdAsync(userId);
}