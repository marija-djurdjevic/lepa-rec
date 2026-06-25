using System;

namespace AngularNetBase.Practice.Entities.Rewards
{
    public class RewardPieceGrant
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid DailySessionId { get; private set; }
        public DateTime SessionDate { get; private set; }
        public Guid RewardProgressId { get; private set; }
        public int PieceIndex { get; private set; }
        public DateTime GrantedAt { get; private set; }
        public UserRewardProgress? RewardProgress { get; private set; }

        private RewardPieceGrant() { }

        public RewardPieceGrant(
            Guid id,
            Guid userId,
            Guid dailySessionId,
            DateTime sessionDate,
            Guid rewardProgressId,
            int pieceIndex,
            DateTime grantedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be provided.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.", nameof(userId));

            if (dailySessionId == Guid.Empty)
                throw new ArgumentException("Daily session id must be provided.", nameof(dailySessionId));

            if (rewardProgressId == Guid.Empty)
                throw new ArgumentException("Reward progress id must be provided.", nameof(rewardProgressId));

            if (pieceIndex is < 0 or >= UserRewardProgress.MaxPieces)
                throw new ArgumentOutOfRangeException(nameof(pieceIndex), "Piece index must be between 0 and 3.");

            Id = id;
            UserId = userId;
            DailySessionId = dailySessionId;
            SessionDate = sessionDate.Date;
            RewardProgressId = rewardProgressId;
            PieceIndex = pieceIndex;
            GrantedAt = grantedAt;
        }
    }
}
