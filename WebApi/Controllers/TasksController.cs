using Application.Services;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IEnumerable<TaskItem>> GetAsync() => await _service.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<TaskItem> GetAsync(Guid id) => await _service.GetByIdAsync(id);
        
        [HttpPost]
        public async Task<IActionResult> AddAsync(string title, string description, DateTime dueDate)
        {
            await _service.AddAsync(title,  description, dueDate);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, string title, string description, DateTime dueDate, bool isDone)
        {
            await _service.UpdateAsync(id, title, description, dueDate, isDone);
            return Ok();
        }
    }
}
