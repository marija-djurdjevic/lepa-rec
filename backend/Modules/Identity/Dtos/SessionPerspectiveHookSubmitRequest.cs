namespace AngularNetBase.Identity.Dtos;

public record SessionPerspectiveHookSubmitRequest(
    Guid OnboardingSessionId,
    Guid ExerciseId,
    IReadOnlyCollection<SessionPerspectiveAnswerItemDto> Answers);
