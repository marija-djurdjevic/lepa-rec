using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AngularNetBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DistancedJournalsController : ControllerBase
    {
        private readonly IDistancedJournalService _distancedJournalService;

        public DistancedJournalsController(IDistancedJournalService distancedJournalService)
        {
            _distancedJournalService = distancedJournalService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet("challenges")]
        public async Task<ActionResult<IEnumerable<DistancedJournalChallengeDto>>> GetAllChallenges(
            CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.GetAllChallengesAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("challenges/level/{challengeLevel}")]
        public async Task<ActionResult<IEnumerable<DistancedJournalChallengeDto>>> GetChallengesByLevel(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.GetChallengesByLevelAsync(challengeLevel, cancellationToken);
            return Ok(result);
        }

        [HttpPost("challenges")]
        public async Task<ActionResult<DistancedJournalChallengeDto>> CreateChallenge(
            [FromBody] CreateDistancedJournalChallengeDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.CreateChallengeAsync(dto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("start")]
        public async Task<ActionResult<DistancedJournalExerciseDto>> StartExercise(
            [FromBody] StartDistancedJournalExerciseDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var command = new StartDistancedJournalExerciseDto(
                userId,
                dto.ChallengeId);

            var result = await _distancedJournalService.StartExerciseAsync(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DistancedJournalExerciseDto>> GetExerciseById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.GetExerciseByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<DistancedJournalExerciseDto>>> GetMyExercises(
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _distancedJournalService.GetExercisesByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("submit")]
        public async Task<ActionResult<DistancedJournalExerciseDto>> SubmitAnswer(
            [FromBody] SubmitDistancedJournalAnswerDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.SubmitAnswerAsync(dto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("reflection")]
        public async Task<ActionResult<DistancedJournalExerciseDto>> AddReflection(
            [FromBody] AddDistancedJournalReflectionDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.AddReflectionAsync(dto, cancellationToken);
            return Ok(result);
        }
    }
}
