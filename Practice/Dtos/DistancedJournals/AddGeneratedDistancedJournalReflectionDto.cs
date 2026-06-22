using System;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public sealed record AddGeneratedDistancedJournalReflectionDto(
        Guid ExerciseId,
        string Answer);
}
