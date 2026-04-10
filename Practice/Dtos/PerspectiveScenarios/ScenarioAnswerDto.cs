using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record ScenarioAnswerDto(
        Guid QuestionId,
        string AnswerText);
}
