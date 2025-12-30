namespace Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public IEnumerable<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<BoardUser> BoardUsers { get; set; }
    public ICollection<BoardInvite> SentInvites { get; set; }
    public ICollection<BoardInvite> ReceivedInvites { get; set; }
}