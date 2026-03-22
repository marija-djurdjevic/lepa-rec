using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioChallengeDto(
        Guid Id,
        string ScenarioText,
        string Reveal,
        ChallengeLevel ChallengeLevel,
        IReadOnlyCollection<PerspectiveScenarioQuestionDto> Questions);
}
