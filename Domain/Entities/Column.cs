namespace Domain;

public class Column
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public Guid BoardId { get; set; } = Guid.NewGuid();
    public Board Board { get; set; } = null!;
    public int Order { get; set; }
    
    public List<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}