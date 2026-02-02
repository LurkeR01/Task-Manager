using Domain;

namespace WebApi.DTOs.Invite;

public class ResponseInviteDto
{
    public Guid InviteId { get; set; }
    public Guid BoardId { get; set; }
    public string BoardTitle { get; set; } = string.Empty;
    public Guid InvitedByUserId { get; set; }
    public string InvitedByUsername { get; set; } = string.Empty;
    public Roles Role { get; set; }
    public DateTime CreatedAt { get; set; }
}