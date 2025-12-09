using WebApi.DTOs.Column;
using WebApi.DTOs.User;

namespace WebApi.DTOs.Board;
 
 public class ResponseBoardDto
 {
     public Guid Id { get; set; } = new Guid();
     public string Title { get; set; } = string.Empty;
     public UserResponseDto Owner { get; set; }  = null!;
     public List<UserDto> Users { get; set; } = new List<UserDto>();
     public List<ColumnDto> Columns { get; set; } = new List<ColumnDto>();
 }