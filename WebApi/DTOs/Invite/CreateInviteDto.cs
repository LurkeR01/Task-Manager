using Domain;

namespace WebApi.DTOs.Invite;

public class CreateInviteDto
{
    public string Email { get; set; }
    public Roles Role { get; set; }
}