namespace WebApi.DTOs;

public class ColumnDto
{
    public Guid Id { get; set; } = new Guid();
    public string Name { get; set; }  = string.Empty;
    public int Order { get; set; }
    public List<CreateTaskDto> Tasks { get; set; } = new List<CreateTaskDto>();
}