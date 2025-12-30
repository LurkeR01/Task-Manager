using System.Security.Claims;
using Application.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using WebApi.DTOs.TaskItem;

namespace WebApi.Controllers
{
    [Route("api/columns")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TasksService _tasksService;

        public TasksController(TasksService tasksService)
        {
            _tasksService = tasksService;
        }

        [Authorize]
        [HttpGet("{columnId}/tasks/{taskItemId}", Name = "GetTaskItem")]
        public async Task<IActionResult> GetAsync(Guid columnId, Guid taskItemId)
        {
            try
            {
                return Ok(await _tasksService.GetByIdAsync(taskItemId, columnId));
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    [Authorize]
        [HttpPost("{columnId}/tasks")]
        public async Task<IActionResult> AddAsync(Guid columnId, [FromBody] TaskItemDto itemDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                var taskItem = await _tasksService.AddAsync(itemDto.Title, 
                    itemDto.Description, 
                    itemDto.DueDate, 
                    columnId, 
                    userId);
                
                TaskItemDto taskItemDto = new TaskItemDto
                {
                    Id = taskItem.Id,
                    Description = taskItem.Description,
                    DueDate = taskItem.DueDate,
                    ColumnId = taskItem.ColumnId
                };
                
                return CreatedAtRoute(
                    "GetTaskItem",
                    new { columnId, taskItemId = taskItemDto.Id },
                    taskItemDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize]
        [HttpPatch("{columnId}/tasks/{taskItemId}")]
        public async Task<IActionResult> UpdateAsync(Guid taskItemId, Guid columnId, [FromBody] UpdateTaskDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                var taskItem = await _tasksService.UpdateAsync(taskItemId, userId, columnId, dto.Title, dto.Description, dto.DueDate, dto.ColumnId);
                TaskItemDto taskItemDto = new TaskItemDto
                {
                    Id = taskItem.Id,
                    Title = taskItem.Title,
                    Description = taskItem.Description,
                    DueDate = taskItem.DueDate,
                    ColumnId = taskItem.ColumnId
                };
                return CreatedAtRoute(
                    "GetTaskItem",
                    new { columnId, taskItemId = taskItemDto.Id },
                    taskItemDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize]
        [HttpDelete("{columnId}/tasks/{taskItemId}")]
        public async Task<IActionResult> DeleteAsync(Guid taskItemId, Guid columnId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                await _tasksService.DeleteAsync(taskItemId, columnId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
