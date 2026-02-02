using Domain;
using WebApi.DTOs.Board;
using WebApi.DTOs.Column;
using WebApi.DTOs.TaskItem;
using WebApi.DTOs.User;

namespace WebApi.Mappers;

public static class BoardMapping
{
    public static ResponseBoardDto ToFullResponse(this Board b)
    {
        return new ResponseBoardDto
        {
            Id = b.Id,
            Title = b.Title,
            Owner = new UserResponseDto
            {
                Id = b.Owner.Id,
                Username = b.Owner.Username
            },
            Members = b.Members.Select(bu => new UserResponseDto
            {
                Id = bu.User.Id,
                Username = bu.User.Username
            }).ToList(),
            MembersCount = b.Members.Count,
            Columns = b.Columns.OrderBy(c => c.Order).Select(c => new ResponseColumnDto
            {
                Id = c.Id,
                Title = c.Title,
                Order = c.Order,
                TaskItems = c.TaskItems.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    ColumnId = t.ColumnId,
                }).ToList()
            }).ToList()
        };
    }

    public static ResponseBoardDto ToShortResponse(this Board b)
    {
        return new ResponseBoardDto
        {
            Id = b.Id,
            Title = b.Title,
            Owner = new UserResponseDto
            {
                Id = b.Owner.Id,
                Username = b.Owner.Username
            },
            MembersCount = b.Members.Count
        };
    }
}