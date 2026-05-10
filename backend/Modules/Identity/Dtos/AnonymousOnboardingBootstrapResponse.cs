namespace AngularNetBase.Identity.Dtos;

public record AnonymousOnboardingBootstrapResponse(
    Guid OnboardingSessionId,
    DateTime ExpiresAt,
    string PreferredLanguage,
    string? HookType,
    Guid? HookChallengeId,
    bool HookExerciseCompleted,
    bool IsUsed);
