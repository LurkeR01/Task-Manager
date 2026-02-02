using System.Security.Claims;
using Application;
using Application.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Board;
using WebApi.Mappers;

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
        public async Task<IActionResult> GetAllAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            var boards = await _boardsService.GetAllAsync(userId);
            
            return Ok(boards.Select(b => b.ToShortResponse()).ToList());
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetBoard")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            var board = await _boardsService.GetByIdAsync(id, userId);
            return Ok(board.ToFullResponse());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateBoardDto boardDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            var createdBoard = await _boardsService.AddAsync(boardDto.Title, userId);
            return CreatedAtRoute(
                "GetBoard",
                new { id = createdBoard.Id },
                createdBoard.ToFullResponse());
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateBoardDto boardDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            var updatedBoard = await _boardsService.UpdateAsync(id, userId, boardDto.Title);
            return CreatedAtRoute(
                "GetBoard",
                new { id = updatedBoard.Id },
                updatedBoard.ToFullResponse());
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            await _boardsService.DeleteAsync(id, userId);
            return NoContent();
        }
    }
}
