using Domain;
using WebApi.DTOs.Column;
using WebApi.DTOs.TaskItem;

namespace WebApi.Mappers;

public static class ColumnMapping
{
    public static ResponseColumnDto ToResponse(this Column c)
    {
        return new ResponseColumnDto
        {
            Id = c.Id,
            Title = c.Title,
            BoardId = c.BoardId,
            Order = c.Order,
            TaskItems = c.TaskItems.Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                ColumnId = t.ColumnId,
            }).ToList()
        };
    }
}