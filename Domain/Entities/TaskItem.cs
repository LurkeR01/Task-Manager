namespace Domain;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsDone { get; set; }
    
    public int? ParentId { get; set; }
    public ParentType ParentType { get; set; }
}