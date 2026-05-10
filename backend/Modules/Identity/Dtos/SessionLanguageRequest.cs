namespace AngularNetBase.Identity.Dtos;

public record SessionLanguageRequest(Guid OnboardingSessionId, string PreferredLanguage);
