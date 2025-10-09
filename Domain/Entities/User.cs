namespace Domain;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    public IEnumerable<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public IEnumerable<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}