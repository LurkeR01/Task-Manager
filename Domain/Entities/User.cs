namespace Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;

    public IEnumerable<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public IEnumerable<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}