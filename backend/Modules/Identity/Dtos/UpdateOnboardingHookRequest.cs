namespace AngularNetBase.Identity.Dtos;

public record UpdateOnboardingHookRequest(string HookType, Guid? HookChallengeId);
