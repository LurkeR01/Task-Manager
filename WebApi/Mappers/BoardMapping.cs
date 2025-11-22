using Domain;
using WebApi.DTOs;

namespace WebApi.Mappers;

public static class BoardMapping
{
    public static BoardDto ToResponse(this Board b)
    {
        return new BoardDto
        {
            Id = b.Id,
            Title = b.Title,
            Owner = new UserDto
            {
                Id = b.Owner.Id,
                Username = b.Owner.Username
            },
            Users = b.BoardUsers.Select(bu => new UserDto
            {
                Id = bu.User.Id,
                Username = bu.User.Username
            }).ToList(),
            Columns = b.Columns.OrderBy(c => c.Order).Select(c => new ColumnDto
            {
                Id = c.Id,
                Name = c.Name,
                Order = c.Order,
                Tasks = c.TaskItems.OrderBy(t => t.IsDone).Select(t => new CreateTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                }).ToList()
            }).ToList()
        };
    }
}