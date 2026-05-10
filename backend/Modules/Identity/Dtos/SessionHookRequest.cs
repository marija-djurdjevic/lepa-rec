namespace AngularNetBase.Identity.Dtos;

public record SessionHookRequest(Guid OnboardingSessionId, string HookType, Guid? HookChallengeId);
