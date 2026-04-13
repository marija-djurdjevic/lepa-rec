using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record DistancedJournalReflectionPromptDto(
        Guid ExerciseId,
        string ChallengeContent,
        string ChallengeFollowUpQuestion,
        string? PreviousMainAnswer,
        string? PreviousFollowUpAnswer,
        IReadOnlyCollection<string> PreviousPhotoUrls);
}
