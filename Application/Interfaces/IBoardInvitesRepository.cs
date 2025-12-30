using Domain;

namespace Application;

public interface IBoardInvitesRepository
{
    Task<BoardInvite?> GetByInvitedUserIdAsync(Guid invitedUserId);
    Task CreateInviteAsync(BoardInvite boardInvite);
}