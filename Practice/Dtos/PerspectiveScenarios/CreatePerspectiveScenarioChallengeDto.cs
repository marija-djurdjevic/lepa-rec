using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record CreatePerspectiveScenarioChallengeDto(
        int ActorCount,
        PerspectiveScenarioContext Context,
        string ScenarioText,
        string Reveal,
        ChallengeLevel ChallengeLevel,
        IReadOnlyCollection<CreatePerspectiveScenarioQuestionDto> Questions);
}
