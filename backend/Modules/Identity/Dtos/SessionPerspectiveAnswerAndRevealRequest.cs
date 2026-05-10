namespace AngularNetBase.Identity.Dtos;

public record SessionPerspectiveAnswerAndRevealRequest(
    Guid OnboardingSessionId,
    Guid ExerciseId,
    Guid QuestionId,
    string AnswerText);
