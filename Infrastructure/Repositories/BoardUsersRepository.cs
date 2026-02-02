using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BoardUsersRepository: IBoardUsersRepository
{
    private readonly AppDbContext _dbContext;
    
    public BoardUsersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<BoardUser> GetByUserIdAsync(Guid userId) => 
        await _dbContext.BoardUsers.FirstOrDefaultAsync(bu => bu.UserId == userId);
    
    public async Task<BoardUser> GetByBoardIdAsync(Guid boardId) =>
        await _dbContext.BoardUsers.FirstOrDefaultAsync(bu => bu.BoardId == boardId);

    public async Task AddBoardUserAsync(BoardUser boardUser)
    {
        await _dbContext.BoardUsers.AddAsync(boardUser);
        await _dbContext.SaveChangesAsync();
    }
}