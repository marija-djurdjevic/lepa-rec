using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record CreatePerspectiveScenarioQuestionDto(
        Guid SkillId,
        int Order,
        string QuestionText,
        string Reveal,
        string? QuestionTextEn = null,
        string? RevealEn = null);
}
