using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitesController : ControllerBase
    {
        private readonly InvitesService _invitesService;
        
        public InvitesController(InvitesService invitesService)
        {
            _invitesService = invitesService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            
            var invites = await _invitesService.GetInvitesForUser(userId);
            return Ok(invites.Select(i => i.ToUserResponse()).ToList());
        }

        [Authorize]
        [HttpPost("{inviteId}")]
        public async Task<IActionResult> AcceptInvite(Guid inviteId)
        {
            await _invitesService.AcceptInvite(inviteId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{inviteId}")]
        public async Task<IActionResult> RejectInvite(Guid inviteId)
        {
            await _invitesService.RejectInvite(inviteId);
            return NoContent();
        }
    }
}
