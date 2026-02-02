using Domain;
using WebApi.DTOs.Invite;

namespace WebApi.Mappers;

public static class BoardInviteMapping
{
    public static ResponseBoardInviteDto ToBoardResponse(this BoardInvite invite)
    {
        return new ResponseBoardInviteDto
        {
            InviteId = invite.Id,
            InvitedUserId = invite.InvitedUserId,
            InvitedUsername = invite.InvitedUser.Username,
            Role = invite.Role,
            CreatedAt = invite.CreatedAt,
            InvitedByUserId = invite.InvitedByUserId,
            InvitedByUsername = invite.InvitedByUser.Username
        };
    }
    
    public static ResponseInviteDto ToUserResponse(this BoardInvite invite)
    {
        return new ResponseInviteDto
        {
            InviteId = invite.Id,
            BoardId = invite.BoardId,
            BoardTitle = invite.Board.Title,
            InvitedByUsername = invite.InvitedByUser.Username,
            InvitedByUserId = invite.InvitedByUserId,
            CreatedAt = invite.CreatedAt,
            Role = invite.Role
        };
    }
}