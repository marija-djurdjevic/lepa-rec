namespace AngularNetBase.Identity.Services;

public static class OnboardingErrorCodes
{
    public const string SessionNotFound = "ONBOARDING_SESSION_NOT_FOUND";
    public const string SessionExpired = "ONBOARDING_SESSION_EXPIRED";
    public const string StepOutOfOrder = "ONBOARDING_STEP_OUT_OF_ORDER";
    public const string Incomplete = "ONBOARDING_INCOMPLETE";
    public const string SessionAlreadyUsed = "ONBOARDING_SESSION_ALREADY_USED";
    public const string InvalidHookType = "INVALID_HOOK_TYPE";
}
