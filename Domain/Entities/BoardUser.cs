namespace Domain;

public class BoardUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BoardId { get; set; } = Guid.Empty;
    public Board Board { get; set; } = null!;
    public Guid UserId { get; set; } = Guid.Empty;
    public User User { get; set; } = null!;
    public Roles Role { get; set; }
}