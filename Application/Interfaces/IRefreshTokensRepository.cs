using Domain;

namespace Application;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> GetByHashAsync(string hash);
    Task AddAsync(RefreshToken refreshToken);
    Task RemoveAsync(RefreshToken refreshToken);
    Task SaveChangesAsync();
}