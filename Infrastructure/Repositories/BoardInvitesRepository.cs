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

    public async Task<IEnumerable<BoardInvite>> GetAllBoardInvites(Guid boardId) =>
        await _dbContext.BoardInvites
            .Include(bi => bi.InvitedUser)
            .Include(bi => bi.InvitedByUser)
            .Where(bi => bi.BoardId == boardId)
            .ToListAsync();
    
    public async Task<IEnumerable<BoardInvite>> GetAllInvitesForUser(Guid userId) =>
        await _dbContext.BoardInvites
            .Include(bi => bi.InvitedByUser)
            .Include(bi => bi.Board)
            .Where(bi => bi.InvitedUserId == userId)
            .ToListAsync();
    
    public async Task<BoardInvite> GetById(Guid inviteId) => 
        await _dbContext.BoardInvites.FirstOrDefaultAsync(bi => bi.Id == inviteId);
    
    public async Task<BoardInvite?> GetByInvitedUserIdAsync(Guid invitedUserId) => 
        await _dbContext.BoardInvites.FirstOrDefaultAsync(bi => bi.InvitedUserId == invitedUserId);
    
    public async Task CreateInviteAsync(BoardInvite boardInvite)
    {
        await _dbContext.BoardInvites.AddAsync(boardInvite);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteInviteAsync(Guid boardInviteId, Guid boardId) =>
        await _dbContext.BoardInvites.Where(bi => bi.Id == boardInviteId && bi.BoardId == boardId).ExecuteDeleteAsync();
}