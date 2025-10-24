using Domain;

namespace Application;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> GetByIdAsync(Guid id);
    Task<RefreshToken?> GetByHashAsync(string hash);
    Task AddAsync(RefreshToken refreshToken);
    Task RemoveAsync(RefreshToken refreshToken);
    Task<IEnumerable<RefreshToken>> GetAllForUserAsync(Guid userId);
    Task SaveChangesAsync();
}