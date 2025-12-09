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

    public async Task<Column> GetByIdAsync(Guid columnId)
    {
        return await _dbContext.Columns.
            Include(c => c.TaskItems)
            .FirstOrDefaultAsync(c => c.Id == columnId);
    }
    
    public async Task AddAsync(Column newColumn)
    {
        await _dbContext.AddAsync(newColumn);
        await _dbContext.SaveChangesAsync();
    }
}