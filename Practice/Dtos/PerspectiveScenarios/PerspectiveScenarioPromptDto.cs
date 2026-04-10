using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioPromptDto(
        Guid Id,
        int ActorCount,
        PerspectiveScenarioContext Context,
        string ScenarioText,
        ChallengeLevel ChallengeLevel,
        IReadOnlyCollection<PerspectiveScenarioQuestionDto> Questions);
}
