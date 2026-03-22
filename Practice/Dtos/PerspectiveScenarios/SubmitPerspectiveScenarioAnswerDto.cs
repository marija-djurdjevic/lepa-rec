using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public sealed record SubmitPerspectiveScenarioAnswerDto(
        Guid ExerciseId,
        DateTime SessionDate,
        IReadOnlyCollection<SubmitPerspectiveScenarioAnswerItemDto> Answers);
}
