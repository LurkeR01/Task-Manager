using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly AppDbContext _dbContext;

    public RefreshTokensRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByIdAsync(Guid id)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByHashAsync(string hash)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(rt => rt.TokenHash == hash);
    }
    
    public async Task AddAsync(RefreshToken refreshToken) => await _dbContext.RefreshTokens.AddAsync(refreshToken);

    public async Task RemoveAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens
            .Where(rt => rt.Id == refreshToken.Id)
            .ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllForUserAsync(Guid userId)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }
    
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}