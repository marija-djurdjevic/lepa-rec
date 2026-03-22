using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record SubmitPerspectiveScenarioAnswerItemDto(
        Guid QuestionId,
        string AnswerText);
}
