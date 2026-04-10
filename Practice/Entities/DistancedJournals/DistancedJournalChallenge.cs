using AngularNetBase.Shared.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalChallenge : Entity<Guid>
    {
        public string Content { get; private set; }
        public string FollowUpQuestion { get; private set; }
        public ChallengeLevel ChallengeLevel { get; private set; }

        private DistancedJournalChallenge() : base()
        {
            Content = string.Empty;
            FollowUpQuestion = string.Empty;
        }

        public DistancedJournalChallenge(Guid id, string content, string followUpQuestion, ChallengeLevel challengeLevel)
            : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must be provided.");

            if (string.IsNullOrWhiteSpace(followUpQuestion))
                throw new ArgumentException("Follow-up question must be provided.");

            Content = content.Trim();
            FollowUpQuestion = followUpQuestion.Trim();
            ChallengeLevel = challengeLevel;
        }

        public void Update(string content, string followUpQuestion, ChallengeLevel challengeLevel)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must be provided.");

            if (string.IsNullOrWhiteSpace(followUpQuestion))
                throw new ArgumentException("Follow-up question must be provided.");

            Content = content.Trim();
            FollowUpQuestion = followUpQuestion.Trim();
            ChallengeLevel = challengeLevel;
        }
    }
}
