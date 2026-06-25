using System;

namespace AngularNetBase.Practice.Dtos.Rewards
{
    public record RewardProgressDto(
        Guid RewardProgressId,
        string RewardImageId,
        string AssetPath,
        string? ImageUrl,
        int UnlockedPiecesCount,
        int? PreviousUnlockedPiecesCount,
        int? NewlyUnlockedPieceIndex,
        bool IsCompleted,
        DateTime? CompletedAt,
        DateTime? SavedAt,
        bool ShouldPlayUnlockAnimation);
}
