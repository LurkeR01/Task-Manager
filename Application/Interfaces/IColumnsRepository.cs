using Domain;

namespace Application;

public interface IColumnsRepository
{
    Task<Column> GetByIdAsync(Guid columnId);
    Task<ICollection<Column>> GetAllByBoardIdAsync(Guid baordId, Guid ownerId);
    Task<Column> GetOneByBoardIdAsync(Guid columnId, Guid boardId);
    Task<Column> GetByIdWithBoardForUserAsync(Guid columnId, Guid userId);
    Task AddAsync(Column newColumn);
    Task UpdateAsync(Column updatedColumn);
    Task DeleteAsync(Guid columnId, Guid ownerId);
    Task SaveChangesAsync();
}