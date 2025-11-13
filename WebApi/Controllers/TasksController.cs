using System.Security.Claims;
using Application.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TasksService _service;

        public TasksController(TasksService service)
        {
            _service = service;
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            return Ok(await _service.GetAllAsync(userId));
        }
    
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id) {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            var task = await _service.GetByIdAsync(id, userId);
             
            return Ok(task);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateTaskDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                await _service.AddAsync(dto.Title, dto.Description, dto.DueDate, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTaskDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                await _service.UpdateAsync(id, userId, dto.Title, dto.Description, dto.DueDate, dto.IsDone);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid taskItemId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("USER ID" + userIdClaim);
            
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                await _service.DeleteAsync(taskItemId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            return Ok(User.Identity.Name);
        }
    }
}
