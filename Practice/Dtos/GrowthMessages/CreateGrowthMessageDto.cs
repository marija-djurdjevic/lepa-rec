using AngularNetBase.Practice.Entities.GrowthMessages;

namespace AngularNetBase.Practice.Dtos.GrowthMessages
{
    public record CreateGrowthMessageDto(
        string Text,
        GrowthMessageType Type = GrowthMessageType.Begin,
        string? TextEn = null);
}
