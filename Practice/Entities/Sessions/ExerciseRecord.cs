using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public record ExerciseRecord(Guid ExerciseId, ExerciseType Type, DateTime Timestamp) : SessionEvent(Timestamp);
}
