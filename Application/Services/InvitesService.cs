using Domain;

namespace Application.Services;

public class InvitesService
{
    private readonly IBoardUsersRepository _boardUsersRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IBoardInvitesRepository _boardInvitesRepository;
    
    public InvitesService(
        IBoardUsersRepository boardUsersRepository,
        IUsersRepository usersRepository,
        IBoardInvitesRepository boardInvitesRepository)
    {
        _boardUsersRepository = boardUsersRepository;
        _usersRepository = usersRepository;
        _boardInvitesRepository = boardInvitesRepository;
    }

    public async Task<BoardInvite> CreateInvite(string email, Guid invitedByUserId, Guid boardId, Roles role)
    {
        var boardUser = await _boardUsersRepository.GetByUserIdAsync(invitedByUserId);
        if (boardUser.Role != Roles.Admin && boardUser.Role != Roles.Owner)
            throw new Exception("Only admin users can create invites");
        
        if (boardUser.Id == invitedByUserId) 
            throw new Exception("You can't invite yourself");
        
        var invitedUser = await _usersRepository.GetByEmailAsync(email) ?? throw new Exception("User not found");

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
}