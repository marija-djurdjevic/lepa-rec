using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record StartPerspectiveScenarioExerciseDto(
        Guid UserId,
        Guid ChallengeId);
}
