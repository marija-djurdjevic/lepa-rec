using AngularNetBase.Shared.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalChallenge : Entity<Guid>
    {
        public string Content { get; private set; }
        public string? ContentEn { get; private set; }
        public string FollowUpQuestion { get; private set; }
        public string? FollowUpQuestionEn { get; private set; }
        public ChallengeLevel ChallengeLevel { get; private set; }
        public Guid? SkillId { get; private set; }

        private DistancedJournalChallenge() : base()
        {
            Content = string.Empty;
            FollowUpQuestion = string.Empty;
        }

        public DistancedJournalChallenge(
            Guid id,
            string content,
            string followUpQuestion,
            ChallengeLevel challengeLevel,
            Guid? skillId = null,
            string? contentEn = null,
            string? followUpQuestionEn = null)
            : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must be provided.");

            if (string.IsNullOrWhiteSpace(followUpQuestion))
                throw new ArgumentException("Follow-up question must be provided.");

            Content = content.Trim();
            ContentEn = NormalizeOptional(contentEn);
            FollowUpQuestion = followUpQuestion.Trim();
            FollowUpQuestionEn = NormalizeOptional(followUpQuestionEn);
            ChallengeLevel = challengeLevel;
            SkillId = skillId;
        }

        public void Update(
            string content,
            string followUpQuestion,
            ChallengeLevel challengeLevel,
            Guid? skillId = null,
            string? contentEn = null,
            string? followUpQuestionEn = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must be provided.");

            if (string.IsNullOrWhiteSpace(followUpQuestion))
                throw new ArgumentException("Follow-up question must be provided.");

            Content = content.Trim();
            ContentEn = NormalizeOptional(contentEn);
            FollowUpQuestion = followUpQuestion.Trim();
            FollowUpQuestionEn = NormalizeOptional(followUpQuestionEn);
            ChallengeLevel = challengeLevel;
            SkillId = skillId;
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
