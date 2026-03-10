using AngularNetBase.Practice.Dtos.Sessions;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AngularNetBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet("today")]
        public async Task<ActionResult<DailySessionStateDto>> GetTodaySession(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _sessionService.GetOrCreateTodaySessionAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("primer/complete")]
        public async Task<ActionResult<DailySessionStateDto>> CompletePrimer(
            [FromBody] CompletePrimerDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _sessionService.CompletePrimerAsync(userId, dto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("exercises/record")]
        public async Task<ActionResult<DailySessionStateDto>> RecordExercise(
            [FromBody] RecordExerciseDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _sessionService.RecordExerciseAsync(userId, dto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("complete")]
        public async Task<ActionResult<DailySessionStateDto>> CompleteSession(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _sessionService.CompleteTodaySessionAsync(userId, cancellationToken);
            return Ok(result);
        }
    }
}
