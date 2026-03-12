using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public sealed record SubmitDistancedJournalAnswerDto(
    Guid ExerciseId,
    DateTime SessionDate,
    string MainAnswer,
    string FollowUpAnswer,
    string? Reflection);
}
