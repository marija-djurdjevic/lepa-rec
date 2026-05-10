namespace AngularNetBase.Identity.Dtos;

public record RegisterWithOnboardingRequest(
    string Email,
    string Password,
    Guid OnboardingSessionId,
    UpdateOnboardingProfileRequest Profile);
