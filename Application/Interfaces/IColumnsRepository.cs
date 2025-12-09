using Domain;

namespace Application;

public interface IColumnsRepository
{
    Task<Column> GetByIdAsync(Guid columnId);
    Task AddAsync(Column newColumn);
}