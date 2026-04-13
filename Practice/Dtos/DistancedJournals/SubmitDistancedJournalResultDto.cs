using AngularNetBase.Practice.Entities.DistancedJournals.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public record SubmitDistancedJournalResultDto(
        DistancedJournalExerciseDto Exercise,
        ThirdPersonFeedbackType? FeedbackType);
}
