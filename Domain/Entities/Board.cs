namespace Domain;

public class Board
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    
    public List<Column> Columns { get; set; } = new List<Column>();
    public ICollection<BoardUser> BoardUsers { get; set; } = new List<BoardUser>();
    
}