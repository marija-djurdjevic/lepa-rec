using System.Security.Claims;
using AngularNetBase.Identity.Dtos;
using AngularNetBase.Identity.Services;
using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetBase.API.Controllers;

[ApiController]
[Route("api/onboarding")]
[Authorize]
public class OnboardingController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IDistancedJournalService _distancedJournalService;
    private readonly IPerspectiveScenarioService _perspectiveScenarioService;

    public OnboardingController(
        AuthService authService,
        IDistancedJournalService distancedJournalService,
        IPerspectiveScenarioService perspectiveScenarioService)
    {
        _authService = authService;
        _distancedJournalService = distancedJournalService;
        _perspectiveScenarioService = perspectiveScenarioService;
    }

    [HttpGet("bootstrap")]
    public async Task<IActionResult> GetBootstrap()
    {
        var userId = GetUserId();
        var result = await _authService.GetBootstrapAsync(userId);
        return Ok(result);
    }

    [HttpPut("language")]
    public async Task<IActionResult> UpdateLanguage(UpdateOnboardingLanguageRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _authService.UpdateOnboardingLanguageAsync(userId, request.PreferredLanguage);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("hook")]
    public async Task<IActionResult> UpdateHook(UpdateOnboardingHookRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _authService.UpdateOnboardingHookAsync(userId, request.HookType, request.HookChallengeId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("hook/distanced-journal-challenge")]
    public async Task<IActionResult> GetDistancedJournalHook([FromQuery] string? lang)
    {
        var result = await _distancedJournalService.GetOnboardingHookChallengeAsync(lang);
        return Ok(result);
    }

    [HttpGet("hook/perspective-scenario-challenge")]
    public async Task<IActionResult> GetPerspectiveScenarioHook([FromQuery] string? lang)
    {
        var result = await _perspectiveScenarioService.GetOnboardingHookChallengeAsync(lang);
        return Ok(result);
    }

    [HttpPost("hook/distanced-journal/start")]
    public async Task<IActionResult> StartDistancedJournalHook([FromBody] StartDistancedJournalExerciseDto request)
    {
        var userId = GetUserId();
        var command = new StartDistancedJournalExerciseDto(userId, request.ChallengeId);
        var result = await _distancedJournalService.StartExerciseAsync(command, isOnboardingHookRun: true);
        return Ok(result);
    }

    [HttpPost("hook/distanced-journal/submit")]
    public async Task<IActionResult> SubmitDistancedJournalHook([FromBody] SubmitDistancedJournalAnswerDto request)
    {
        var userId = GetUserId();
        var result = await _distancedJournalService.SubmitAnswerAsync(
            userId,
            request,
            trackInDailySession: false);
        return Ok(result);
    }

    [HttpPost("hook/perspective-scenario/start")]
    public async Task<IActionResult> StartPerspectiveScenarioHook([FromBody] StartPerspectiveScenarioExerciseDto request)
    {
        var userId = GetUserId();
        var command = new StartPerspectiveScenarioExerciseDto(userId, request.ChallengeId);
        var result = await _perspectiveScenarioService.StartExerciseAsync(command, isOnboardingHookRun: true);
        return Ok(result);
    }

    [HttpPost("hook/perspective-scenario/submit")]
    public async Task<IActionResult> SubmitPerspectiveScenarioHook(
        [FromBody] SubmitPerspectiveScenarioAnswerDto request,
        [FromQuery] string? lang)
    {
        var userId = GetUserId();
        var result = await _perspectiveScenarioService.SubmitAnswersAsync(
            userId,
            request,
            lang,
            trackInDailySession: false);
        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateOnboardingProfileRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _authService.UpdateOnboardingProfileAsync(
                userId,
                request.FirstName,
                request.LastName,
                request.NotificationEnabled,
                request.NotificationTimeLocal,
                request.TimeZoneId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete()
    {
        try
        {
            var userId = GetUserId();
            await _authService.CompleteOnboardingAsync(userId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidOperationException("Authenticated user id is missing.");

        return userId;
    }
}
