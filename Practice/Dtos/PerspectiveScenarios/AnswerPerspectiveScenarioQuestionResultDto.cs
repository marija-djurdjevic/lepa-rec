namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public sealed record AnswerPerspectiveScenarioQuestionResultDto(
        PerspectiveScenarioExerciseDto Exercise,
        PerspectiveScenarioRevealDto Reveal,
        bool IsExerciseCompleted,
        int AnsweredQuestionsCount,
        int TotalQuestions);
}
