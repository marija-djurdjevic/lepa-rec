namespace AngularNetBase.Identity.Dtos;

public record UserBootstrapResponse(
    string UserId,
    bool OnboardingCompleted,
    string PreferredLanguage,
    string FirstName,
    string LastName,
    bool NotificationEnabled,
    string? NotificationTimeLocal,
    string? TimeZoneId,
    string? HookType,
    Guid? HookChallengeId);
