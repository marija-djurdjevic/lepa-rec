using AngularNetBase.API.Models;
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
            var userId = GetUserId();
            var result = await _distancedJournalService.GetExerciseByIdAsync(userId, id, cancellationToken);

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
        public async Task<ActionResult<SubmitDistancedJournalResultDto>> SubmitAnswer(
            [FromBody] SubmitDistancedJournalAnswerDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            try
            {
                var result = await _distancedJournalService.SubmitAnswerAsync(userId, dto, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpPost("submit-with-photos")]
        [RequestSizeLimit(20_000_000)]
        public async Task<ActionResult<SubmitDistancedJournalResultDto>> SubmitAnswerWithPhotos(
            [FromForm] SubmitDistancedJournalPhotoAnswerRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var dto = new SubmitDistancedJournalAnswerDto(
                request.ExerciseId,
                request.SessionDate,
                request.MainAnswer ?? string.Empty,
                request.FollowUpAnswer ?? string.Empty,
                request.Reflection);

            var photos = request.Photos
                .Select(p => new PhotoUpload(
                    p.OpenReadStream(),
                    p.ContentType ?? "application/octet-stream",
                    p.FileName,
                    p.Length))
                .ToList();

            try
            {
                var result = await _distancedJournalService.SubmitAnswerWithPhotosAsync(
                    userId,
                    dto,
                    photos,
                    cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpPost("reflection")]
        public async Task<ActionResult<DistancedJournalExerciseDto>> AddReflection(
            [FromBody] AddDistancedJournalReflectionDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            try
            {
                var result = await _distancedJournalService.AddReflectionAsync(userId, dto, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpGet("challenges/random/{level}")]
        public async Task<ActionResult<DistancedJournalChallengeDto>> GetRandomChallenge(ChallengeLevel level, CancellationToken cancellationToken)
        {
            var result = await _distancedJournalService.GetRandomChallengeAsync(level, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}/photos/{photoId:guid}")]
        public async Task<IActionResult> GetPhoto(
            Guid id,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            try
            {
                var result = await _distancedJournalService.GetPhotoAsync(userId, id, photoId, cancellationToken);
                return File(result.Stream, result.ContentType, result.FileName);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }
    }
}
