namespace Domain;

public class Column
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public int Order { get; set; }
    
    public List<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}