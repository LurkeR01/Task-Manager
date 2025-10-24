using Application.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAsync() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id) {
             var task = await _service.GetByIdAsync(id);
             if (task == null) return NotFound();
             
             return Ok(task);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateTaskDto dto)
        {
            await _service.AddAsync(dto.Title, dto.Description, dto.DueDate);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTaskDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto.Title, dto.Description, dto.DueDate, dto.IsDone);
                return Ok();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (ArgumentException)
            {
                return NotFound();
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
