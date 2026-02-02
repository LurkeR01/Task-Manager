using Domain;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Services;

public class InvitesService
{
    private readonly IBoardUsersRepository _boardUsersRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IBoardInvitesRepository _boardInvitesRepository;
    private readonly IUnitOfWork _uof;
    
    public InvitesService(
        IBoardUsersRepository boardUsersRepository,
        IUsersRepository usersRepository,
        IBoardInvitesRepository boardInvitesRepository,
        IUnitOfWork uof)
    {
        _boardUsersRepository = boardUsersRepository;
        _usersRepository = usersRepository;
        _boardInvitesRepository = boardInvitesRepository;
        _uof = uof;
    }

    public async Task<IEnumerable<BoardInvite>> GetBoardInvites(Guid boardId, Guid userId)
    {
        if (await _boardUsersRepository.GetByUserIdAsync(userId) == null) 
            throw new ForbiddenException("You are not member of this board");
        
        return await _boardInvitesRepository.GetAllBoardInvites(boardId);
    }

    public async Task<IEnumerable<BoardInvite>> GetInvitesForUser(Guid userId) =>
        await _boardInvitesRepository.GetAllInvitesForUser(userId);

    public async Task<BoardInvite> CreateInvite(string email, Guid invitedByUserId, Guid boardId, Roles role)
    {
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(invitedByUserId);
        if (boardUser.Role != Roles.Admin && boardUser.Role != Roles.Owner)
            throw new ForbiddenException("Only admin users can create invites");
        
        if (boardUser.Id == invitedByUserId) 
            throw new Exception("You can't invite yourself");
        
        var invitedUser = await _usersRepository.GetByEmailAsync(email) ?? throw new NotFoundException("User not found");

        if (await _boardInvitesRepository.GetByInvitedUserIdAsync(invitedUser.Id) != null)
            throw new Exception("User is already invited");
        
        BoardInvite invite = new BoardInvite
        {
            BoardId = boardId,
            InvitedByUserId = invitedByUserId,
            InvitedUserId = invitedUser.Id,
            Role = role
        };
        
        _boardInvitesRepository.CreateInviteAsync(invite);
        return invite;
    }

    public async Task RevokeInvite(Guid inviteId, Guid boardId, Guid userId)
    {
        if (await _boardInvitesRepository.GetById(inviteId) == null) throw new NotFoundException("Invite not found");
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(userId) ?? throw new ForbiddenException("You are not member of this board");
        
        if (boardUser.Role != Roles.Admin && boardUser.Role != Roles.Owner)
            throw new ForbiddenException("You are not allowed to revoke invite");

        await _boardInvitesRepository.DeleteInviteAsync(inviteId, boardId);
    }

    public async Task AcceptInvite(Guid inviteId)
    {
        await _uof.BeginTransactionAsync();
        
        try
        {
            var invite = await _boardInvitesRepository.GetById(inviteId);

            BoardUser newBoardUser = new BoardUser
            {
                BoardId = invite.BoardId,
                UserId = invite.InvitedUserId,
                Role = invite.Role
            };
            await _boardUsersRepository.AddBoardUserAsync(newBoardUser);
            await _boardInvitesRepository.DeleteInviteAsync(invite.Id, invite.BoardId);
            
            await _uof.CommitAsync();
        }
        catch
        {
            await _uof.RollbackAsync();
            throw;
        }
    }

    public async Task RejectInvite(Guid inviteId)
    {
        var invite = await _boardInvitesRepository.GetById(inviteId) ?? throw new NotFoundException("Invite not found");
        await _boardInvitesRepository.DeleteInviteAsync(invite.Id, invite.BoardId);
    }
}