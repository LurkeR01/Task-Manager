using WebApi.DTOs.TaskItem;

namespace WebApi.DTOs.Column;

public class ResponseColumnDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid BoardId { get; set; }
    public int Order { get; set; }
    public List<TaskItemDto> TaskItems { get; set; } = new List<TaskItemDto>();
}