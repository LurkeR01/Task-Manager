using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Column;
using WebApi.Mappers;

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
        [HttpGet("{boardId}/columns/{columnId}", Name = "GetColumn")]
        public async Task<ActionResult> GetByIdAsync(Guid columnId, Guid boardId)
        {
            var column = await _columnsService.GetByIdAsync(columnId, boardId);
            return Ok(column.ToResponse());
        }
        
        
        [Authorize]
        [HttpPost("{boardId}/columns")]
        public async Task<IActionResult> AddAsync(Guid boardId, [FromBody] ColumnDto columnDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            var createdColumn = await _columnsService.AddAsync(columnDto.Title, boardId, userId);
               
            return CreatedAtRoute(
                "GetColumn",
                new
                {
                    boardId = boardId,
                    columnId = createdColumn.Id 
                },
                createdColumn.ToResponse());
        }

        [Authorize]
        [HttpPatch("{boardId}/columns/{columnId}")]
        public async Task<IActionResult> UpdateAsync(Guid boardId, Guid columnId, [FromBody] ColumnDto columnDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            var updatedColumn = await _columnsService.UpdateAsync(columnId, userId, boardId, columnDto.Title, columnDto.Order);
                
            return CreatedAtRoute(
                "GetColumn",
                new
                {
                    columnId = updatedColumn.Id,
                    boardId = boardId,
                },
                updatedColumn.ToResponse());
        }

        [Authorize]
        [HttpDelete("{boardId}/columns/{columnId}")]
        public async Task<IActionResult> DeleteAsync(Guid boardId, Guid columnId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            await _columnsService.DeleteAsync(columnId, userId, boardId);
            return NoContent();
        }
    }
}
