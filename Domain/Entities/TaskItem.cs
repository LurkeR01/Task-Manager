namespace Domain;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsDone { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}