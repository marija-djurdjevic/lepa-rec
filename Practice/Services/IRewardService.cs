using AngularNetBase.Practice.Dtos.Rewards;
using AngularNetBase.Practice.Entities.Sessions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngularNetBase.Practice.Services
{
    public interface IRewardService
    {
        Task<RewardProgressDto?> GetCurrentRewardAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<RewardProgressDto?> GrantDailyPieceAsync(
            Guid userId,
            DailySession session,
            CancellationToken cancellationToken = default);

        Task<RewardProgressDto> SaveRewardAsync(
            Guid userId,
            Guid rewardProgressId,
            CancellationToken cancellationToken = default);

        Task<RewardGalleryDto> GetGalleryAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
