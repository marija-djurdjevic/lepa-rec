using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public record AddDistancedJournalReflectionDto(
    Guid ExerciseId,
    string Reflection);
}
