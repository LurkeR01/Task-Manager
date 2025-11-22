using System.Security.Claims;
using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly BoardsService _boardsService;

        public BoardsController(BoardsService boardsService)
        {
            _boardsService = boardsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            return Ok(await _boardsService.GetAllAsync(userId));
        }
    }
}
