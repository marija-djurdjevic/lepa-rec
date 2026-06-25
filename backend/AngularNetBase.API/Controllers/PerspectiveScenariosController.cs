using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace AngularNetBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PerspectiveScenariosController : ControllerBase
    {
        private readonly IPerspectiveScenarioService _perspectiveScenarioService;
        private static readonly JsonSerializerOptions StreamJsonOptions = new(JsonSerializerDefaults.Web);

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
            [FromQuery] string? lang,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetAllChallengesAsync(lang, cancellationToken);
            return Ok(result);
        }

        [HttpGet("challenges/level/{challengeLevel}")]
        public async Task<ActionResult<IEnumerable<PerspectiveScenarioChallengeDto>>> GetChallengesByLevel(
            ChallengeLevel challengeLevel,
            [FromQuery] string? lang,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetChallengesByLevelAsync(challengeLevel, lang, cancellationToken);
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

            var result = await _perspectiveScenarioService.StartExerciseAsync(
                command,
                cancellationToken: cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PerspectiveScenarioExerciseDto>> GetExerciseById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _perspectiveScenarioService.GetExerciseByIdAsync(userId, id, cancellationToken);

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
            [FromQuery] string? lang,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            try
            {
                var result = await _perspectiveScenarioService.SubmitAnswersAsync(
                    userId,
                    dto,
                    lang,
                    cancellationToken: cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpPost("answer-and-reveal")]
        public async Task<ActionResult<AnswerPerspectiveScenarioQuestionResultDto>> AnswerQuestionAndGetReveal(
            [FromBody] AnswerPerspectiveScenarioQuestionDto dto,
            [FromQuery] string? lang,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            try
            {
                var result = await _perspectiveScenarioService.AnswerQuestionAndGetRevealAsync(
                    userId,
                    dto,
                    lang,
                    cancellationToken: cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpPost("answer-and-reveal/stream")]
        public async Task AnswerQuestionAndStreamGuidance(
            [FromBody] AnswerPerspectiveScenarioQuestionDto dto,
            [FromQuery] string? lang,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            Response.ContentType = "text/event-stream";
            Response.Headers.CacheControl = "no-cache";
            Response.Headers.Connection = "keep-alive";

            try
            {
                var result = await _perspectiveScenarioService.AnswerQuestionAndStreamGuidanceAsync(
                    userId,
                    dto,
                    async (grade, token) =>
                    {
                        await WriteSseEventAsync(
                            "grade",
                            new
                            {
                                score = grade.Score,
                                issues = grade.Issues,
                                strengths = grade.Strengths,
                                language = grade.Language
                            },
                            token);
                    },
                    async (chunk, token) =>
                    {
                        await WriteSseEventAsync(
                            "guide_chunk",
                            new { chunk },
                            token);
                    },
                    lang,
                    cancellationToken: cancellationToken);

                await WriteSseEventAsync(
                    "final",
                    new PerspectiveScenarioStreamFinalEventDto(result),
                    cancellationToken);
            }
            catch (UnauthorizedAccessException)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                await WriteSseEventAsync(
                    "error",
                    new { message = "Perspective scenario exercise was not found." },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await WriteSseEventAsync(
                    "error",
                    new { message = ex.Message },
                    cancellationToken);
            }
        }

        [HttpGet("challenges/random/{level}")]
        public async Task<ActionResult<PerspectiveScenarioPromptDto>> GetRandomChallenge(
            ChallengeLevel level,
            [FromQuery] string? lang,
            CancellationToken cancellationToken)
        {
            var result = await _perspectiveScenarioService.GetRandomChallengeAsync(level, lang, cancellationToken);
            return Ok(result);
        }

        private async Task WriteSseEventAsync(
            string eventName,
            object payload,
            CancellationToken cancellationToken)
        {
            await Response.WriteAsync($"event: {eventName}\n", cancellationToken);
            await Response.WriteAsync($"data: {JsonSerializer.Serialize(payload, StreamJsonOptions)}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
