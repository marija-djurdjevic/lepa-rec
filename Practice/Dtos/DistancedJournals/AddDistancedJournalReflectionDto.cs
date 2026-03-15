using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public sealed record AddDistancedJournalReflectionDto(
        Guid ExerciseId,
        DateTime SessionDate,
        string Reflection);
}
