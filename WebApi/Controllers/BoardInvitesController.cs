using System.Security.Claims;
using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApi.DTOs.Invite;

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
        [HttpPost]
        public async Task<IActionResult> CreateInvite(Guid boardId, [FromBody] CreateInviteDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            try
            {
                var createdInvite = await _invitesService.CreateInvite(dto.Email, userId, boardId, dto.Role);
                return Ok(createdInvite);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
