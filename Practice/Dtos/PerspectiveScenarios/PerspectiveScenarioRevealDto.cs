using System;

namespace AngularNetBase.Practice.Dtos.PerspectiveScenarios
{
    public record PerspectiveScenarioRevealDto(
        Guid QuestionId,
        int Order,
        string Reveal);
}
