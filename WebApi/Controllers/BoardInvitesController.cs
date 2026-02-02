using System.Security.Claims;
using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApi.DTOs.Invite;
using WebApi.Mappers;

namespace WebApi.Controllers
{
    [Route("api/boards/{boardId}/invites")]
    [ApiController]
    public class BoardInvitesController : ControllerBase
    {
        private readonly InvitesService _invitesService;

        public BoardInvitesController(InvitesService invitesService)
        {
            _invitesService = invitesService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid boardId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            var invites = await _invitesService.GetBoardInvites(boardId, userId);
            return Ok(invites.Select(bi => bi.ToBoardResponse()).ToList());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateInvite(Guid boardId, [FromBody] CreateInviteDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            var createdInvite = await _invitesService.CreateInvite(dto.Email, userId, boardId, dto.Role);
            return Ok(createdInvite.ToBoardResponse());
        }

        [Authorize]
        [HttpDelete("{inviteId}")]
        public async Task<IActionResult> DeleteInvite(Guid boardId, Guid inviteId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            await _invitesService.RevokeInvite(inviteId, boardId, userId);
            return NoContent();
        }
    }
}
