using System.Security.Claims;
using AngularNetBase.Identity.Dtos;
using AngularNetBase.Identity.Services;
using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetBase.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly OnboardingSessionService _onboardingSessionService;
    private readonly IDistancedJournalService _distancedJournalService;
    private readonly IPerspectiveScenarioService _perspectiveScenarioService;

    public AuthController(
        AuthService authService,
        OnboardingSessionService onboardingSessionService,
        IDistancedJournalService distancedJournalService,
        IPerspectiveScenarioService perspectiveScenarioService)
    {
        _authService = authService;
        _onboardingSessionService = onboardingSessionService;
        _distancedJournalService = distancedJournalService;
        _perspectiveScenarioService = perspectiveScenarioService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        if (result is null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register-with-onboarding")]
    public async Task<IActionResult> RegisterWithOnboarding(RegisterWithOnboardingRequest request)
    {
        try
        {
            var session = await _onboardingSessionService.GetActiveSessionAsync(request.OnboardingSessionId);
            _onboardingSessionService.EnsureReadyForRegistration(session);

            var user = await _authService.FindUserByEmailAsync(request.Email);
            if (user is null)
            {
                user = await _authService.RegisterUserAsync(request.Email, request.Password);
            }
            else
            {
                if (user.OnboardingCompleted)
                    return BadRequest(new { message = "Email is already registered." });

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var callerUserId) || callerUserId != user.Id)
                {
                    return Unauthorized(new { message = "Existing account onboarding requires an authenticated session for that account." });
                }
            }

            await _authService.UpdateOnboardingLanguageAsync(user.Id, session.PreferredLanguage!);
            await _authService.UpdateOnboardingHookAsync(user.Id, session.HookType!, session.HookChallengeId);

            if (session.HookType == "distancedjournal")
            {
                if (!session.HookChallengeId.HasValue || !session.DistancedSessionDate.HasValue
                    || string.IsNullOrWhiteSpace(session.DistancedMainAnswer)
                    || string.IsNullOrWhiteSpace(session.DistancedFollowUpAnswer))
                {
                    return BadRequest(new { code = OnboardingErrorCodes.Incomplete, message = "Onboarding session is incomplete." });
                }

                var started = await _distancedJournalService.StartExerciseAsync(
                    new StartDistancedJournalExerciseDto(user.Id, session.HookChallengeId.Value),
                    isOnboardingHookRun: true);

                await _distancedJournalService.SubmitAnswerAsync(
                    user.Id,
                    new SubmitDistancedJournalAnswerDto(
                        started.Id,
                        session.DistancedSessionDate.Value,
                        session.DistancedMainAnswer!,
                        session.DistancedFollowUpAnswer!,
                        session.DistancedReflection),
                    trackInDailySession: false);
            }
            else if (session.HookType == "perspectivescenario")
            {
                if (!session.HookChallengeId.HasValue || string.IsNullOrWhiteSpace(session.PerspectiveAnswersJson))
                    return BadRequest(new { code = OnboardingErrorCodes.Incomplete, message = "Onboarding session is incomplete." });

                var answers = System.Text.Json.JsonSerializer.Deserialize<IReadOnlyCollection<SessionPerspectiveAnswerItemDto>>(
                    session.PerspectiveAnswersJson!);

                if (answers is null || answers.Count == 0)
                    return BadRequest(new { code = OnboardingErrorCodes.Incomplete, message = "Onboarding session is incomplete." });

                var started = await _perspectiveScenarioService.StartExerciseAsync(
                    new StartPerspectiveScenarioExerciseDto(user.Id, session.HookChallengeId.Value),
                    isOnboardingHookRun: true);

                await _perspectiveScenarioService.SubmitAnswersAsync(
                    user.Id,
                    new SubmitPerspectiveScenarioAnswerDto(
                        started.Id,
                        DateTime.UtcNow,
                        answers.Select(x => new SubmitPerspectiveScenarioAnswerItemDto(x.QuestionId, x.AnswerText)).ToList()),
                    session.PerspectiveLang,
                    trackInDailySession: false);
            }

            await _authService.UpdateOnboardingProfileAsync(
                user.Id,
                request.Profile.FirstName,
                request.Profile.LastName,
                request.Profile.NotificationEnabled,
                request.Profile.NotificationTimeLocal,
                request.Profile.TimeZoneId);

            await _authService.CompleteOnboardingAsync(user.Id);
            await _onboardingSessionService.MarkUsedAsync(session.Id);

            var response = await _authService.IssueTokensAsync(user);
            return Ok(response);
        }
        catch (OnboardingException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken);
        if (result is null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _authService.LogoutAsync(request.RefreshToken, userId);
        return NoContent();
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var result = await _authService.LoginWithGoogleAsync(request.IdToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid Google Token" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
