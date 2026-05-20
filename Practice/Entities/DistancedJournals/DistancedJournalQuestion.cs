using System;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalQuestion
    {
        public Guid Id { get; private set; }
        public Guid DistancedJournalChallengeId { get; private set; }
        public DistancedJournalQuestionKind Kind { get; private set; }
        public int Order { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public string? TextEn { get; private set; }
        public Guid? SkillId { get; private set; }

        private DistancedJournalQuestion() { }

        public DistancedJournalQuestion(
            Guid id,
            Guid distancedJournalChallengeId,
            DistancedJournalQuestionKind kind,
            int order,
            string text,
            Guid? skillId = null,
            string? textEn = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Question id must be provided.", nameof(id));

            if (distancedJournalChallengeId == Guid.Empty)
                throw new ArgumentException("Challenge id must be provided.", nameof(distancedJournalChallengeId));

            if (order < 1)
                throw new ArgumentException("Question order must be positive.", nameof(order));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Question text must be provided.", nameof(text));

            Id = id;
            DistancedJournalChallengeId = distancedJournalChallengeId;
            Kind = kind;
            Order = order;
            Text = text.Trim();
            TextEn = NormalizeOptional(textEn);
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
