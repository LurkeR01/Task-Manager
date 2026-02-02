using Domain;

namespace WebApi.DTOs.Invite;

public class ResponseBoardInviteDto
{
    public Guid InviteId { get; set; }
    public Guid InvitedUserId { get; set; }
    public string InvitedUsername { get; set; } = string.Empty;
    public Roles Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid InvitedByUserId { get; set; }
    public string InvitedByUsername { get; set; }  = string.Empty;
}