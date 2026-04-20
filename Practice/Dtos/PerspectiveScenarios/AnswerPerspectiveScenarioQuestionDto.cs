namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public sealed record AnswerPerspectiveScenarioQuestionDto(
        Guid ExerciseId,
        Guid QuestionId,
        string AnswerText);
}
