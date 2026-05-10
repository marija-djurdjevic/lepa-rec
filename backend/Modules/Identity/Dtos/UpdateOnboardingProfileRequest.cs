namespace AngularNetBase.Identity.Dtos;

public record UpdateOnboardingProfileRequest(
    string FirstName,
    string LastName,
    bool NotificationEnabled,
    string? NotificationTimeLocal,
    string? TimeZoneId);
