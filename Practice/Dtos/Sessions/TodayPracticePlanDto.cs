using AngularNetBase.Practice.Dtos.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record TodayPracticePlanDto(
       DistancedJournalReflectionPromptDto? ReflectionPrompt,
       IReadOnlyCollection<DistancedJournalChallengeDto> DistancedJournalChoices,
       bool ShouldShowPerspectiveScenario,
       bool IsDistancedJournalCompleted,
       bool IsReflectionCompleted,
       bool IsPerspectiveScenarioCompleted);
}
