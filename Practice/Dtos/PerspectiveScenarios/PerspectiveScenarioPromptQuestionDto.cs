using System;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioPromptQuestionDto(
        Guid Id,
        Guid SkillId,
        int Order,
        string QuestionText);
}
