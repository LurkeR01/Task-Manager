using WebApi.DTOs.TaskItem;

namespace WebApi.DTOs.Column;

public class ColumnDto
{
    public Guid Id { get; set; } = new Guid();
    public string Name { get; set; }  = string.Empty;
    public int Order { get; set; }
    public List<TaskItemDto> Tasks { get; set; } = new List<TaskItemDto>();
}