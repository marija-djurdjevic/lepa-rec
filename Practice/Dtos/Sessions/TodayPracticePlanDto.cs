using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record TodayPracticePlanDto(
       DistancedJournalReflectionPromptDto? ReflectionPrompt,
       IReadOnlyCollection<DistancedJournalChallengeDto> DistancedJournalChoices,
       IReadOnlyCollection<PerspectiveScenarioPromptDto> PerspectiveScenarioChoices,
       bool ShouldShowPerspectiveScenario,
       bool IsDistancedJournalCompleted,
       bool IsReflectionCompleted,
       bool IsPerspectiveScenarioCompleted);
}
