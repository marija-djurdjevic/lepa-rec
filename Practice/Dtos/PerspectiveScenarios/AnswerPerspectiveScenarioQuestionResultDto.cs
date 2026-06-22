namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public sealed record AnswerPerspectiveScenarioQuestionResultDto(
        PerspectiveScenarioExerciseDto Exercise,
        PerspectiveScenarioRevealDto? Reveal,
        bool IsExerciseCompleted,
        int AnsweredQuestionsCount,
        int TotalQuestions,
        string Status = PerspectiveScenarioAnswerStatus.Completed,
        int? Grade = null,
        IReadOnlyCollection<string>? Issues = null,
        IReadOnlyCollection<string>? Strengths = null,
        PerspectiveScenarioGuideQuestionDto? GuideQuestion = null,
        int GuideIterationCount = 0,
        string? Feedback = null);
}
