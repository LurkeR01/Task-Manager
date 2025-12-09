using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Column;

namespace WebApi.Controllers
{
    [Route("api/boards")]
    [ApiController]
    public class ColumnsController : ControllerBase
    {
        private readonly ColumnsService _columnsService;

        public ColumnsController(ColumnsService columnsService)
        {
            _columnsService = columnsService;
        }

        [Authorize]
        [HttpGet("columns/{columnId}", Name = "GetColumn")]
        public async Task<ActionResult> GetByIdAsync(Guid columnId) => Ok(await _columnsService.GetByIdAsync(columnId));
        
        
        [Authorize]
        [HttpPost("{boardId}/columns")]
        public async Task<IActionResult> AddAsync(Guid boardId, [FromBody] ColumnDto columnDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                var createdColumn = await _columnsService.AddAsync(columnDto.Name, boardId, userId);
                ColumnDto createdColumnDto = new ColumnDto
                {
                    Id = createdColumn.Id,
                    Name = createdColumn.Name,
                    Order = createdColumn.Order
                };
                
                return CreatedAtRoute(
                    "GetColumn",
                    new { columnId = createdColumn.Id },
                    createdColumnDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
    }
}
