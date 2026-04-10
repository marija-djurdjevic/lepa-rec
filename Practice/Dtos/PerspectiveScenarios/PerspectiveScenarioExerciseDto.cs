using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioExerciseDto(
        Guid Id,
        Guid UserId,
        Guid ChallengeId,
        IReadOnlyCollection<ScenarioAnswerDto> Answers,
        DateTime? SubmittedAt,
        bool IsCompleted);
}
