using Domain;

namespace Application;

public interface IColumnsRepository
{
    Task AddAsync(Column newColumn);
}