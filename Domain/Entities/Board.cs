namespace Domain;

public class Board
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }  = string.Empty;
    public Guid OwnerId { get; set; } = Guid.Empty;
    public User Owner { get; set; } = null!;
    
    public List<Column> Columns { get; set; } = new List<Column>();
    public ICollection<BoardUser> Members { get; set; } = new List<BoardUser>();
    public ICollection<BoardInvite> BoardInvites { get; set; } = new List<BoardInvite>();
}