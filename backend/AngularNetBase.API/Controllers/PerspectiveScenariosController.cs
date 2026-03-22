using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
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
    public class PerspectiveScenariosController : ControllerBase
    {
        private readonly IPerspectiveScenarioService _perspectiveScenarioService;

        public PerspectiveScenariosController(IPerspectiveScenarioService perspectiveScenarioService)
        {
            _perspectiveScenarioService = perspectiveScenarioService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet("challenges")]
        public async Task<ActionResult<IEnumerable<PerspectiveScenarioChallengeDto>>> GetAllChallenges(
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetAllChallengesAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("challenges/level/{challengeLevel}")]
        public async Task<ActionResult<IEnumerable<PerspectiveScenarioChallengeDto>>> GetChallengesByLevel(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetChallengesByLevelAsync(challengeLevel, cancellationToken);
            return Ok(result);
        }

        [HttpPost("challenges")]
        public async Task<ActionResult<PerspectiveScenarioChallengeDto>> CreateChallenge(
            [FromBody] CreatePerspectiveScenarioChallengeDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.CreateChallengeAsync(dto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("start")]
        public async Task<ActionResult<PerspectiveScenarioExerciseDto>> StartExercise(
            [FromBody] StartPerspectiveScenarioExerciseDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var command = new StartPerspectiveScenarioExerciseDto(
                userId,
                dto.ChallengeId);

            var result = await _perspectiveScenarioService.StartExerciseAsync(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PerspectiveScenarioExerciseDto>> GetExerciseById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetExerciseByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<PerspectiveScenarioExerciseDto>>> GetMyExercises(
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _perspectiveScenarioService.GetExercisesByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("submit")]
        public async Task<ActionResult<SubmitPerspectiveScenarioResultDto>> SubmitAnswers(
            [FromBody] SubmitPerspectiveScenarioAnswerDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.SubmitAnswersAsync(dto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("challenges/random/{level}")]
        public async Task<ActionResult<PerspectiveScenarioPromptDto>> GetRandomChallenge(
            ChallengeLevel level,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetRandomChallengeAsync(level, cancellationToken);
            return Ok(result);
        }
    }
}
