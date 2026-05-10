namespace AngularNetBase.Identity.Dtos;

public record SessionSubmitDistancedHookRequest(
    Guid OnboardingSessionId,
    Guid ExerciseId,
    DateTime SessionDate,
    string MainAnswer,
    string FollowUpAnswer,
    string? Reflection);
