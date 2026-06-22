using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public record DistancedJournalExerciseDto(
    Guid Id,
    Guid UserId,
    Guid ChallengeId,
    string? MainAnswer,
    string? FollowUpAnswer,
    string? Reflection,
    string? GeneratedReflectionQuestion,
    string? GeneratedReflectionAnswer,
    DateTime? SubmittedAt,
    bool IsCompleted,
    bool IsOnboardingHookRun,
    IReadOnlyCollection<string> PhotoUrls);
}
