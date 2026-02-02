using WebApi.DTOs.Column;
using WebApi.DTOs.User;

namespace WebApi.DTOs.Board;
 
 public class ResponseBoardDto
 {
     public Guid Id { get; set; } = new Guid();
     public string Title { get; set; } = string.Empty;
     public UserResponseDto Owner { get; set; }  = null!;
     public List<UserResponseDto> Members { get; set; } = new List<UserResponseDto>();
     public int MembersCount { get; set; }
     public List<ResponseColumnDto> Columns { get; set; } = new List<ResponseColumnDto>();
 }