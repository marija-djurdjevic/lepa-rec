namespace AngularNetBase.Identity.Dtos;

public record StartOnboardingSessionResponse(Guid OnboardingSessionId, DateTime ExpiresAt);
