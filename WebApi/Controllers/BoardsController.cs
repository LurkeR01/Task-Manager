using System.Security.Claims;
using Application;
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

        [Authorize]
        [HttpGet("{id}", Name = "GetBoard")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            return Ok(await _boardsService.GetByIdAsync(id, userId));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateBoardDto boardDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                var createdBoard = await _boardsService.AddAsync(boardDto.Title, userId);
                return CreatedAtRoute(
                    "GetBoard",
                    new { id = createdBoard.Id },
                    createdBoard);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await _boardsService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
