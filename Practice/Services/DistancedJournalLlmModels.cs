using System.Collections.Generic;

namespace AngularNetBase.Practice.Services
{
    public sealed record DistancedJournalQuestionInput(
        string Language,
        string OpeningQuestion,
        string FollowUpQuestion,
        string? ReflectionQuestion,
        string MainAnswer,
        string FollowUpAnswer);

    public sealed record DistancedJournalGeneratedQuestionResult(string Question);

    public interface IDistancedJournalLlmClient
    {
        Task<DistancedJournalGeneratedQuestionResult> GenerateReflectionQuestionAsync(
            DistancedJournalQuestionInput input,
            IReadOnlyCollection<string> avoidQuestions,
            CancellationToken cancellationToken = default);
    }
}
