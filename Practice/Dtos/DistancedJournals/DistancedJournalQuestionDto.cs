using AngularNetBase.Practice.Entities.DistancedJournals;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
    public record DistancedJournalQuestionDto(
        Guid Id,
        DistancedJournalQuestionKind Kind,
        int Order,
        string Text,
        Guid? SkillId);
}
