using Domain;

namespace Application;

public interface IBoardInvitesRepository
{
    Task <IEnumerable<BoardInvite>> GetAllBoardInvites(Guid boardId);
    Task <IEnumerable<BoardInvite>> GetAllInvitesForUser(Guid userId);
    Task <BoardInvite> GetById(Guid inviteId);
    Task<BoardInvite?> GetByInvitedUserIdAsync(Guid invitedUserId);
    Task CreateInviteAsync(BoardInvite boardInvite);
    Task DeleteInviteAsync(Guid boardInviteId, Guid boardId);
}