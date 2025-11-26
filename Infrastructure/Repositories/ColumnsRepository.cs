using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ColumnsRepository : IColumnsRepository
{
    private readonly AppDbContext _context;

    public ColumnsRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Column newColumn)
    {
        await _context.AddAsync(newColumn);
        await _context.SaveChangesAsync();
    }
}