using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record CreatePerspectiveScenarioQuestionDto(
        Guid SkillId,
        string QuestionText);
}
