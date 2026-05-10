using AngularNetBase.Identity.Dtos;
using AngularNetBase.Identity.Services;
using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetBase.API.Controllers;

[ApiController]
[Route("api/onboarding/session")]
public class OnboardingSessionController : ControllerBase
{
    private readonly OnboardingSessionService _sessionService;
    private readonly IDistancedJournalService _distancedJournalService;
    private readonly IPerspectiveScenarioService _perspectiveScenarioService;

    public OnboardingSessionController(
        OnboardingSessionService sessionService,
        IDistancedJournalService distancedJournalService,
        IPerspectiveScenarioService perspectiveScenarioService)
    {
        _sessionService = sessionService;
        _distancedJournalService = distancedJournalService;
        _perspectiveScenarioService = perspectiveScenarioService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartOnboardingSessionRequest request)
    {
        var response = await _sessionService.StartSessionAsync(request.DeviceFingerprint);
        return Ok(response);
    }

    [HttpGet("bootstrap")]
    public async Task<IActionResult> Bootstrap([FromQuery] Guid sessionId)
    {
        try
        {
            var result = await _sessionService.GetBootstrapAsync(sessionId);
            return Ok(result);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPut("language")]
    public async Task<IActionResult> SetLanguage([FromBody] SessionLanguageRequest request)
    {
        try
        {
            await _sessionService.SetLanguageAsync(request.OnboardingSessionId, request.PreferredLanguage);
            return NoContent();
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPut("hook")]
    public async Task<IActionResult> SetHook([FromBody] SessionHookRequest request)
    {
        try
        {
            await _sessionService.SetHookAsync(request.OnboardingSessionId, request.HookType, request.HookChallengeId);
            return NoContent();
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpGet("hook/distanced-journal-challenge")]
    public async Task<IActionResult> GetDistancedHook([FromQuery] Guid sessionId, [FromQuery] string? lang)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(sessionId);
            if (session.HookType != "distancedjournal")
                throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Select distanced journal hook first.");

            var result = await _distancedJournalService.GetOnboardingHookChallengeAsync(lang);
            return Ok(result);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpGet("hook/perspective-scenario-challenge")]
    public async Task<IActionResult> GetPerspectiveHook([FromQuery] Guid sessionId, [FromQuery] string? lang)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(sessionId);
            if (session.HookType != "perspectivescenario")
                throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Select perspective scenario hook first.");

            var result = await _perspectiveScenarioService.GetOnboardingHookChallengeAsync(lang);
            return Ok(result);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPost("hook/distanced-journal/start")]
    public async Task<IActionResult> StartDistanced([FromBody] SessionStartHookExerciseRequest request)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(request.OnboardingSessionId);
            if (session.HookType != "distancedjournal")
                throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Select distanced journal hook first.");

            var started = await _distancedJournalService.StartExerciseAsync(
                new StartDistancedJournalExerciseDto(session.Id, request.ChallengeId),
                isOnboardingHookRun: true);

            await _sessionService.SaveDistancedHookProgressAsync(session.Id, request.ChallengeId, started.Id);
            return Ok(started);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPost("hook/distanced-journal/submit")]
    public async Task<IActionResult> SubmitDistanced([FromBody] SessionSubmitDistancedHookRequest request)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(request.OnboardingSessionId);
            var result = await _distancedJournalService.SubmitAnswerAsync(
                session.Id,
                new SubmitDistancedJournalAnswerDto(
                    request.ExerciseId,
                    request.SessionDate,
                    request.MainAnswer,
                    request.FollowUpAnswer,
                    request.Reflection),
                trackInDailySession: false);

            await _sessionService.MarkDistancedHookCompletedAsync(request.OnboardingSessionId, request);
            return Ok(result);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPost("hook/perspective-scenario/start")]
    public async Task<IActionResult> StartPerspective([FromBody] SessionStartHookExerciseRequest request)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(request.OnboardingSessionId);
            if (session.HookType != "perspectivescenario")
                throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Select perspective scenario hook first.");

            var started = await _perspectiveScenarioService.StartExerciseAsync(
                new StartPerspectiveScenarioExerciseDto(session.Id, request.ChallengeId),
                isOnboardingHookRun: true);

            await _sessionService.SavePerspectiveHookProgressAsync(session.Id, request.ChallengeId, started.Id);
            return Ok(started);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPost("hook/perspective-scenario/submit")]
    public async Task<IActionResult> SubmitPerspective([FromBody] SessionPerspectiveHookSubmitRequest request, [FromQuery] string? lang)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(request.OnboardingSessionId);
            var dto = new SubmitPerspectiveScenarioAnswerDto(
                request.ExerciseId,
                DateTime.UtcNow,
                request.Answers.Select(x => new SubmitPerspectiveScenarioAnswerItemDto(x.QuestionId, x.AnswerText)).ToList());
            var result = await _perspectiveScenarioService.SubmitAnswersAsync(
                session.Id,
                dto,
                lang,
                trackInDailySession: false);

            await _sessionService.MarkPerspectiveHookCompletedAsync(request.OnboardingSessionId, request, lang);
            return Ok(result);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPost("hook/perspective-scenario/answer-and-reveal")]
    public async Task<IActionResult> AnswerPerspectiveAndReveal(
        [FromBody] SessionPerspectiveAnswerAndRevealRequest request,
        [FromQuery] string? lang)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionAsync(request.OnboardingSessionId);
            if (session.HookType != "perspectivescenario")
                throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Select perspective scenario hook first.");

            var result = await _perspectiveScenarioService.AnswerQuestionAndGetRevealAsync(
                session.Id,
                new AnswerPerspectiveScenarioQuestionDto(
                    request.ExerciseId,
                    request.QuestionId,
                    request.AnswerText),
                lang,
                trackInDailySession: false);

            var answers = result.Exercise.Answers
                .Select(x => new SessionPerspectiveAnswerItemDto(x.QuestionId, x.AnswerText))
                .ToList();

            await _sessionService.SavePerspectiveHookAnswerProgressAsync(
                request.OnboardingSessionId,
                request.ExerciseId,
                answers,
                lang,
                result.IsExerciseCompleted);

            return Ok(result);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
    }
}
