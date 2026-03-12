using AngularNetBase.Practice.Entities.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record RecordExerciseDto(Guid ExerciseId, ExerciseType Type);

}
