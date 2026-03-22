using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record CreatePerspectiveScenarioChallengeDto(
        string ScenarioText,
        string Reveal,
        ChallengeLevel ChallengeLevel,
        IReadOnlyCollection<CreatePerspectiveScenarioQuestionDto> Questions);
}
