using System;

namespace AngularNetBase.Practice.Entities.Scheduling
{
    public class UserChallengeExposure
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public ChallengeExposureType Type { get; private set; }
        public Guid ChallengeId { get; private set; }
        public DateTime ShownOnDate { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private UserChallengeExposure() { }

        public UserChallengeExposure(
            Guid id,
            Guid userId,
            ChallengeExposureType type,
            Guid challengeId,
            DateTime shownOnDate,
            DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be provided.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.", nameof(userId));

            if (challengeId == Guid.Empty)
                throw new ArgumentException("Challenge id must be provided.", nameof(challengeId));

            Id = id;
            UserId = userId;
            Type = type;
            ChallengeId = challengeId;
            ShownOnDate = shownOnDate.Date;
            CreatedAt = createdAt;
        }
    }
}
