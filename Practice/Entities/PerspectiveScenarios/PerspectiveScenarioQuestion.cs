using AngularNetBase.Shared.Core.Domain;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class PerspectiveScenarioQuestion : Entity<Guid>
    {
        public Guid PerspectiveScenarioChallengeId { get; private set; }
        public Guid SkillId { get; private set; }
        public int Order { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public string? QuestionTextEn { get; private set; }
        public string Reveal { get; private set; } = string.Empty;
        public string? RevealEn { get; private set; }

        private PerspectiveScenarioQuestion() : base() { }

        internal PerspectiveScenarioQuestion(
            Guid id,
            Guid perspectiveScenarioChallengeId,
            Guid skillId,
            int order,
            string questionText,
            string reveal,
            string? questionTextEn = null,
            string? revealEn = null) : base(id)
        {
            if (perspectiveScenarioChallengeId == Guid.Empty)
                throw new ArgumentException("Perspective scenario challenge id must be provided.", nameof(perspectiveScenarioChallengeId));

            if (skillId == Guid.Empty)
                throw new ArgumentException("Skill id must be provided.", nameof(skillId));

            if (order < 1)
                throw new ArgumentException("Order must be at least 1.", nameof(order));

            if (string.IsNullOrWhiteSpace(questionText))
                throw new ArgumentException("Question text must be provided.", nameof(questionText));

            if (string.IsNullOrWhiteSpace(reveal))
                throw new ArgumentException("Reveal must be provided.", nameof(reveal));

            PerspectiveScenarioChallengeId = perspectiveScenarioChallengeId;
            SkillId = skillId;
            Order = order;
            QuestionText = questionText.Trim();
            QuestionTextEn = NormalizeOptional(questionTextEn);
            Reveal = reveal.Trim();
            RevealEn = NormalizeOptional(revealEn);
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
