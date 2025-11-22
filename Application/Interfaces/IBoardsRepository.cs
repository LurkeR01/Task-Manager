using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Application;

public interface IBoardsRepository
{
    Task<IEnumerable<Board>> GetAllByUserIdAsync(Guid userId);
    Task AddAsync(Board newBoard);
    Task UpdateAsync(Board updatedBoard);
    Task DeleteAsync(Board deletedBoard);
}