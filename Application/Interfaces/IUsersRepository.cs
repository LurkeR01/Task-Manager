using Domain;

namespace Application;

public interface IUsersRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}