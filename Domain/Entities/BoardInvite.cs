namespace Domain;

public class BoardInvite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BoardId { get; set; } = Guid.Empty;
    public Board Board { get; set; } = null!;
    public Guid InvitedUserId { get; set; } = Guid.Empty;
    public User InvitedUser { get; set; } = null!;
    public Guid InvitedByUserId { get; set; } = Guid.Empty;
    public User InvitedByUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Roles Role { get; set; }
    public Statuses Status { get; set; } = Statuses.Pending;
}