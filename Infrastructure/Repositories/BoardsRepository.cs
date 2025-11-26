using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BoardsRepository : IBoardsRepository
{
    private readonly AppDbContext _dbContext;

    public BoardsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Board>> GetAllByUserIdAsync(Guid userId)
    {
        var boards = await _dbContext.Boards
            .Where(b => b.OwnerId == userId 
            || b.BoardUsers.Any(bu => bu.UserId == userId))
            .Include(b => b.Owner)
            .Include(b => b.BoardUsers)
                .ThenInclude(bu => bu.User)
            .Include(b => b.Columns)
                .ThenInclude(c => c.TaskItems)
            .ToListAsync();
        
        return boards;
    }

    public async Task<Board> GetOneByUserIdAsync(Guid boardId, Guid ownerId)
    {
        return await _dbContext.Boards.FirstOrDefaultAsync(b => b.Id == boardId && b.OwnerId == ownerId);
    }

    public async Task AddAsync(Board newBoard)
    {
        await _dbContext.Boards.AddAsync(newBoard);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Board updatedBoard)
    {
        await _dbContext.Boards.Where(b => b.Id == updatedBoard.Id)
            .ExecuteUpdateAsync(b => b
                .SetProperty(s => s.Title, updatedBoard.Title));
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid boardId)
    {
        await _dbContext.Boards.Where(b => b.Id == boardId)
            .ExecuteDeleteAsync();
    }
}