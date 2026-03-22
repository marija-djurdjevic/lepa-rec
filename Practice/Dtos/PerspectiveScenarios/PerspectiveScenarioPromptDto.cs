using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioPromptDto(
        Guid Id,
        string ScenarioText,
        ChallengeLevel ChallengeLevel,
        IReadOnlyCollection<PerspectiveScenarioQuestionDto> Questions);
}
