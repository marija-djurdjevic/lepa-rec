using System;

namespace AngularNetBase.Practice.Entities.Scheduling
{
    public class DailyChallengeAssignment
    {
        public Guid Id { get; private set; }
        public DateTime Date { get; private set; }
        public Guid DistancedJournalChallengeId { get; private set; }
        public Guid DistancedJournalChallengeId2 { get; private set; }
        public Guid PerspectiveScenarioChallengeId { get; private set; }
        public Guid PerspectiveScenarioChallengeId2 { get; private set; }
        public DateTime AssignedAt { get; private set; }

        private DailyChallengeAssignment() { }

        public DailyChallengeAssignment(
            Guid id,
            DateTime date,
            Guid distancedJournalChallengeId,
            Guid distancedJournalChallengeId2,
            Guid perspectiveScenarioChallengeId,
            Guid perspectiveScenarioChallengeId2,
            DateTime assignedAt)
        {
            Id = id;
            Date = date.Date;
            DistancedJournalChallengeId = distancedJournalChallengeId;
            DistancedJournalChallengeId2 = distancedJournalChallengeId2;
            PerspectiveScenarioChallengeId = perspectiveScenarioChallengeId;
            PerspectiveScenarioChallengeId2 = perspectiveScenarioChallengeId2;
            AssignedAt = assignedAt;
        }

        public void EnsureSecondOptions(Guid distancedJournalChallengeId2, Guid perspectiveScenarioChallengeId2)
        {
            if (DistancedJournalChallengeId2 == Guid.Empty)
            {
                DistancedJournalChallengeId2 = distancedJournalChallengeId2;
            }

            if (PerspectiveScenarioChallengeId2 == Guid.Empty)
            {
                PerspectiveScenarioChallengeId2 = perspectiveScenarioChallengeId2;
            }
        }
    }
}
