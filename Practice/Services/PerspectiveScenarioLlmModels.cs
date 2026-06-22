using AngularNetBase.Practice.Entities.PerspectiveScenarios;

namespace AngularNetBase.Practice.Services
{
    public sealed record PerspectiveScenarioLlmInput(
        string Scenario,
        string Question,
        string Reveal,
        string LearnerAnswer,
        string TargetLanguage,
        IReadOnlyCollection<ConversationTurn> History);

    public sealed record PerspectiveScenarioGradeResult(
        int Score,
        IReadOnlyCollection<string> Issues,
        IReadOnlyCollection<string> Strengths,
        string Language);

    public sealed record PerspectiveScenarioGuideResult(
        string NextQuestion,
        string? WhyThisQuestion);
}
