namespace Domain;

public class BoardUser
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Roles Role { get; set; }
}