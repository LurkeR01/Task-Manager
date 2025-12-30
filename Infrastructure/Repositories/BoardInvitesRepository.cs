using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BoardInvitesRepository : IBoardInvitesRepository
{
    private readonly AppDbContext _dbContext;

    public BoardInvitesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<BoardInvite?> GetByInvitedUserIdAsync(Guid invitedUserId) => 
        await _dbContext.BoardInvites.FirstOrDefaultAsync(bi => bi.InvitedUserId == invitedUserId);
    
    public async Task CreateInviteAsync(BoardInvite boardInvite)
    {
        await _dbContext.BoardInvites.AddAsync(boardInvite);
        await _dbContext.SaveChangesAsync();
    }
}