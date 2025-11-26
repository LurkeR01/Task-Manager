namespace WebApi.DTOs;

public class ResponseBoardDto
{
    public Guid Id { get; set; } = new Guid();
    public string Title { get; set; } = string.Empty;
    public UserDto Owner { get; set; }  = null!;
    public List<UserDto> Users { get; set; } = new List<UserDto>();
    public List<ColumnDto> Columns { get; set; } = new List<ColumnDto>();
}