using WebApi.DTOs.TaskItem;

namespace WebApi.DTOs.Column;

public class ColumnDto
{
    public Guid Id { get; set; } = new Guid();
    public string Title { get; set; }  = string.Empty;
    public int Order { get; set; }
}