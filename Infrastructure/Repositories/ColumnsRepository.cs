using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ColumnsRepository : IColumnsRepository
{
    private readonly AppDbContext _dbContext;

    public ColumnsRepository(AppDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<ICollection<Column>> GetAllByBoardIdAsync(Guid boardId, Guid ownerId) => await _dbContext.Columns
        .Where(c => c.BoardId == boardId && c.Board.OwnerId == ownerId)
        .ToListAsync();

    public async Task<Column> GetOneByBoardIdAsync(Guid columnId, Guid boardId)
    {
        return await _dbContext.Columns.
            Include(c => c.TaskItems)
            .FirstOrDefaultAsync(c => c.Id == columnId && c.Board.Id == boardId);
    }

    public async Task<Column> GetByIdWithBoardForUserAsync(Guid columnId, Guid userId)
    {
        return await _dbContext.Columns
            .Include(c => c.Board)
            .Where(c =>
                c.Id == columnId &&
                c.Board.OwnerId == userId)
            .SingleOrDefaultAsync();
    }
    
    public async Task AddAsync(Column newColumn)
    {
        await _dbContext.AddAsync(newColumn);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Column updatedColumn)
    {
        _dbContext.Columns.Where(c => c.Id == updatedColumn.Id && c.Board.OwnerId == updatedColumn.Board.OwnerId)
            .ExecuteUpdateAsync(b => b
                .SetProperty(s => s.Title, updatedColumn.Title)
                .SetProperty(s => s.Order, updatedColumn.Order));
    }

    public async Task DeleteAsync(Guid columnId, Guid ownerId)
    {
        await _dbContext.Columns.Where(c => c.Id == columnId && c.Board.OwnerId == ownerId)
            .ExecuteDeleteAsync();
    }
    
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}