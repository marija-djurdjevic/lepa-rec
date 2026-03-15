using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public record StartDistancedJournalExerciseDto(
    Guid UserId,
    Guid ChallengeId);
}
