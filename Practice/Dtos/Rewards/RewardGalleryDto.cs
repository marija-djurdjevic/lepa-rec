using System;
using System.Collections.Generic;

namespace AngularNetBase.Practice.Dtos.Rewards
{
    public record RewardGalleryDto(IReadOnlyCollection<RewardGalleryItemDto> CompletedRewards);

    public record RewardGalleryItemDto(
        Guid RewardProgressId,
        string RewardImageId,
        string AssetPath,
        string? ImageUrl,
        DateTime CompletedAt,
        DateTime SavedAt);
}
