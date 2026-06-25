using AngularNetBase.Practice.Dtos.Rewards;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record CompleteSessionResultDto(
        DailySessionStateDto Session,
        RewardProgressDto? CurrentReward);
}
