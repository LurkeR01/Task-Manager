namespace Domain;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    
    [Obsolete]
    public bool IsDone { get; set; } = false;
    
    [Obsolete]
    public Guid UserId { get; set; }
    
    public Guid ColumnId { get; set; }
    public Column Column { get; set; } = null!;
}