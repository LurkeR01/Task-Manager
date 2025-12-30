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
        [HttpGet("{boardId}/columns/{columnId}", Name = "GetColumn")]
        public async Task<ActionResult> GetByIdAsync(Guid columnId, Guid boardId)
        {
            try
            {
                return Ok(await _columnsService.GetByIdAsync(columnId, boardId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
        [Authorize]
        [HttpPost("{boardId}/columns")]
        public async Task<IActionResult> AddAsync(Guid boardId, [FromBody] ColumnDto columnDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                var createdColumn = await _columnsService.AddAsync(columnDto.Title, boardId, userId);
                ColumnDto createdColumnDto = new ColumnDto
                {
                    Id = createdColumn.Id,
                    Title = createdColumn.Title,
                    Order = createdColumn.Order
                };
                
                return CreatedAtRoute(
                    "GetColumn",
                    new
                    {
                        boardId = boardId,
                        columnId = createdColumn.Id 
                    },
                    createdColumnDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("{boardId}/columns/{columnId}")]
        public async Task<IActionResult> UpdateAsync(Guid boardId, Guid columnId, [FromBody] ColumnDto columnDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                var updatedColumn = await _columnsService.UpdateAsync(columnId, userId, columnDto.Title, columnDto.Order);
                ColumnDto updatedColumnDto = new ColumnDto
                {
                    Id = updatedColumn.Id,
                    Title = updatedColumn.Title,
                    Order = updatedColumn.Order
                };

                return CreatedAtRoute(
                    "GetColumn",
                    new { columnId = updatedColumn.Id },
                    updatedColumnDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{boardId}/columns/{columnId}")]
        public async Task<IActionResult> DeleteAsync(Guid boardId, Guid columnId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                await _columnsService.DeleteAsync(columnId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
