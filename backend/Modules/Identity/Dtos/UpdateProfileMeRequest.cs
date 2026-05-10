namespace AngularNetBase.Identity.Dtos;

public record UpdateProfileMeRequest(
    string FirstName,
    string LastName,
    string PreferredLanguage,
    bool NotificationEnabled,
    string? NotificationTimeLocal,
    string? TimeZoneId);
