namespace AngularNetBase.Practice.Services
{
    public interface IPerspectiveScenarioLlmClient
    {
        Task<PerspectiveScenarioGradeResult> GradeAnswerAsync(
            PerspectiveScenarioLlmInput input,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioGuideResult> GenerateGuideQuestionAsync(
            PerspectiveScenarioLlmInput input,
            PerspectiveScenarioGradeResult grade,
            int iterationIndex,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioGuideResult> StreamGuideQuestionAsync(
            PerspectiveScenarioLlmInput input,
            PerspectiveScenarioGradeResult grade,
            int iterationIndex,
            Func<string, CancellationToken, Task> onQuestionChunk,
            CancellationToken cancellationToken = default);
    }
}
