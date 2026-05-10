namespace AngularNetBase.Identity.Dtos;

public record ProfileMeResponse(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    string PreferredLanguage,
    bool NotificationEnabled,
    string? NotificationTimeLocal,
    string? TimeZoneId,
    bool OnboardingCompleted);
