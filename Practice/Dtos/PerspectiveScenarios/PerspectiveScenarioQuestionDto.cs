using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioQuestionDto(
        Guid Id,
        Guid SkillId,
        int Order,
        string QuestionText,
        string Reveal);
}
