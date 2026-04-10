using AngularNetBase.Shared.Core.Domain;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class PerspectiveScenarioQuestion : Entity<Guid>
    {
        public Guid PerspectiveScenarioChallengeId { get; private set; }
        public Guid SkillId { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;

        private PerspectiveScenarioQuestion() : base() { }

        internal PerspectiveScenarioQuestion(
            Guid id,
            Guid perspectiveScenarioChallengeId,
            Guid skillId,
            string questionText) : base(id)
        {
            if (perspectiveScenarioChallengeId == Guid.Empty)
                throw new ArgumentException("Perspective scenario challenge id must be provided.", nameof(perspectiveScenarioChallengeId));

            if (skillId == Guid.Empty)
                throw new ArgumentException("Skill id must be provided.", nameof(skillId));

            if (string.IsNullOrWhiteSpace(questionText))
                throw new ArgumentException("Question text must be provided.", nameof(questionText));

            PerspectiveScenarioChallengeId = perspectiveScenarioChallengeId;
            SkillId = skillId;
            QuestionText = questionText.Trim();
        }
    }
}
