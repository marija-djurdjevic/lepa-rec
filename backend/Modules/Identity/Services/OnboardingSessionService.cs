using AngularNetBase.Identity.Dtos;
using AngularNetBase.Identity.Entities;
using AngularNetBase.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AngularNetBase.Identity.Services;

public class OnboardingSessionService
{
    private static readonly TimeSpan SessionTtl = TimeSpan.FromHours(24);
    private readonly IdentityContext _db;

    public OnboardingSessionService(IdentityContext db)
    {
        _db = db;
    }

    public async Task<StartOnboardingSessionResponse> StartSessionAsync(string? deviceFingerprint)
    {
        var now = DateTime.UtcNow;
        var session = new OnboardingSession
        {
            Id = Guid.NewGuid(),
            CreatedAt = now,
            ExpiresAt = now.Add(SessionTtl),
            DeviceFingerprint = Normalize(deviceFingerprint)
        };

        _db.OnboardingSessions.Add(session);
        await _db.SaveChangesAsync();

        return new StartOnboardingSessionResponse(session.Id, session.ExpiresAt);
    }

    public async Task<AnonymousOnboardingBootstrapResponse> GetBootstrapAsync(Guid sessionId)
    {
        var session = await GetActiveSessionAsync(sessionId);
        return new AnonymousOnboardingBootstrapResponse(
            session.Id,
            session.ExpiresAt,
            session.PreferredLanguage ?? string.Empty,
            session.HookType,
            session.HookChallengeId,
            session.HookExerciseCompleted,
            session.UsedAt.HasValue);
    }

    public async Task<OnboardingSession> GetActiveSessionAsync(Guid sessionId)
    {
        var session = await _db.OnboardingSessions.FirstOrDefaultAsync(x => x.Id == sessionId);
        if (session is null)
            throw new OnboardingException(OnboardingErrorCodes.SessionNotFound, "Onboarding session was not found.");

        if (session.UsedAt.HasValue)
            throw new OnboardingException(OnboardingErrorCodes.SessionAlreadyUsed, "Onboarding session is already used.");

        if (session.ExpiresAt <= DateTime.UtcNow)
            throw new OnboardingException(OnboardingErrorCodes.SessionExpired, "Onboarding session has expired.");

        return session;
    }

    public async Task SetLanguageAsync(Guid sessionId, string preferredLanguage)
    {
        var session = await GetActiveSessionAsync(sessionId);
        session.PreferredLanguage = preferredLanguage.Trim();
        await _db.SaveChangesAsync();
    }

    public async Task SetHookAsync(Guid sessionId, string hookType, Guid? hookChallengeId)
    {
        var session = await GetActiveSessionAsync(sessionId);
        EnsureLanguageStep(session);

        var normalized = hookType.Trim().ToLowerInvariant();
        if (normalized is not ("distancedjournal" or "perspectivescenario"))
            throw new OnboardingException(OnboardingErrorCodes.InvalidHookType, "HookType must be DistancedJournal or PerspectiveScenario.");

        session.HookType = normalized;
        session.HookChallengeId = hookChallengeId;
        await _db.SaveChangesAsync();
    }

    public async Task SaveDistancedHookProgressAsync(Guid sessionId, Guid challengeId, Guid exerciseId)
    {
        var session = await GetActiveSessionAsync(sessionId);
        EnsureHookSelected(session, "distancedjournal");

        session.HookChallengeId = challengeId;
        session.DistancedExerciseId = exerciseId;
        await _db.SaveChangesAsync();
    }

    public async Task SavePerspectiveHookProgressAsync(Guid sessionId, Guid challengeId, Guid exerciseId)
    {
        var session = await GetActiveSessionAsync(sessionId);
        EnsureHookSelected(session, "perspectivescenario");

        session.HookChallengeId = challengeId;
        session.PerspectiveExerciseId = exerciseId;
        await _db.SaveChangesAsync();
    }

    public async Task MarkDistancedHookCompletedAsync(Guid sessionId, SessionSubmitDistancedHookRequest request)
    {
        var session = await GetActiveSessionAsync(sessionId);
        EnsureHookSelected(session, "distancedjournal");
        if (session.DistancedExerciseId != request.ExerciseId)
            throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Hook exercise start step is missing or mismatched.");

        session.DistancedSessionDate = request.SessionDate;
        session.DistancedMainAnswer = request.MainAnswer;
        session.DistancedFollowUpAnswer = request.FollowUpAnswer;
        session.DistancedReflection = request.Reflection;
        session.HookExerciseCompleted = true;

        await _db.SaveChangesAsync();
    }

    public async Task MarkPerspectiveHookCompletedAsync(Guid sessionId, SessionPerspectiveHookSubmitRequest request, string? lang)
    {
        var session = await GetActiveSessionAsync(sessionId);
        EnsureHookSelected(session, "perspectivescenario");
        if (session.PerspectiveExerciseId != request.ExerciseId)
            throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Hook exercise start step is missing or mismatched.");

        session.PerspectiveAnswersJson = System.Text.Json.JsonSerializer.Serialize(request.Answers);
        session.PerspectiveLang = Normalize(lang);
        session.HookExerciseCompleted = true;

        await _db.SaveChangesAsync();
    }

    public async Task SavePerspectiveHookAnswerProgressAsync(
        Guid sessionId,
        Guid exerciseId,
        IReadOnlyCollection<SessionPerspectiveAnswerItemDto> answers,
        string? lang,
        bool isCompleted)
    {
        var session = await GetActiveSessionAsync(sessionId);
        EnsureHookSelected(session, "perspectivescenario");
        if (session.PerspectiveExerciseId != exerciseId)
            throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Hook exercise start step is missing or mismatched.");

        session.PerspectiveAnswersJson = System.Text.Json.JsonSerializer.Serialize(answers);
        session.PerspectiveLang = Normalize(lang);
        session.HookExerciseCompleted = isCompleted;

        await _db.SaveChangesAsync();
    }

    public void EnsureReadyForRegistration(OnboardingSession session)
    {
        EnsureLanguageStep(session);
        EnsureHookSelectionStep(session);
        if (!session.HookExerciseCompleted)
            throw new OnboardingException(OnboardingErrorCodes.Incomplete, "Onboarding hook exercise is not completed.");
    }

    public async Task MarkUsedAsync(Guid sessionId)
    {
        var session = await _db.OnboardingSessions.FirstAsync(x => x.Id == sessionId);
        session.UsedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private static void EnsureLanguageStep(OnboardingSession session)
    {
        if (string.IsNullOrWhiteSpace(session.PreferredLanguage))
            throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Language step must be completed first.");
    }

    private static void EnsureHookSelectionStep(OnboardingSession session)
    {
        if (string.IsNullOrWhiteSpace(session.HookType))
            throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Hook selection step must be completed first.");
    }

    private static void EnsureHookSelected(OnboardingSession session, string expectedHookType)
    {
        EnsureHookSelectionStep(session);
        if (!string.Equals(session.HookType, expectedHookType, StringComparison.OrdinalIgnoreCase))
            throw new OnboardingException(OnboardingErrorCodes.StepOutOfOrder, "Selected hook type does not match this endpoint.");
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
