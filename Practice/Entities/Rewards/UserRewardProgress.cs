using System;

namespace AngularNetBase.Practice.Entities.Rewards
{
    public class UserRewardProgress
    {
        public const int MaxPieces = 4;

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid RewardImageId { get; private set; }
        public int UnlockedPiecesCount { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? SavedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public RewardImage? RewardImage { get; private set; }

        public bool IsCompleted => CompletedAt.HasValue;

        private UserRewardProgress() { }

        public UserRewardProgress(
            Guid id,
            Guid userId,
            Guid rewardImageId,
            DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be provided.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.", nameof(userId));

            if (rewardImageId == Guid.Empty)
                throw new ArgumentException("Reward image id must be provided.", nameof(rewardImageId));

            Id = id;
            UserId = userId;
            RewardImageId = rewardImageId;
            UnlockedPiecesCount = 0;
            CreatedAt = createdAt;
            UpdatedAt = createdAt;
        }

        public int UnlockNextPiece(DateTime timestamp)
        {
            if (UnlockedPiecesCount >= MaxPieces)
                return MaxPieces - 1;

            var pieceIndex = UnlockedPiecesCount;
            UnlockedPiecesCount += 1;
            UpdatedAt = timestamp;

            if (UnlockedPiecesCount == MaxPieces)
                CompletedAt = timestamp;

            return pieceIndex;
        }

        public void Save(DateTime timestamp)
        {
            if (!CompletedAt.HasValue)
                throw new InvalidOperationException("Only completed rewards can be saved.");

            SavedAt ??= timestamp;
            UpdatedAt = timestamp;
        }
    }
}
