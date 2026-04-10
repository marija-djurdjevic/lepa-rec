using AngularNetBase.Shared.Core.Domain;

namespace AngularNetBase.Practice.Entities.Skills
{
    public class SkillMastery : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid SkillId { get; private set; }
        public int CurrentLevel { get; private set; }

        private SkillMastery() : base() { }

        public SkillMastery(Guid id, Guid userId, Guid skillId, int currentLevel) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be a valid GUID.", nameof(userId));

            if (skillId == Guid.Empty)
                throw new ArgumentException("Skill id must be a valid GUID.", nameof(skillId));

            if (currentLevel <= 0)
                throw new ArgumentException("Current level must be greater than zero.", nameof(currentLevel));

            UserId = userId;
            SkillId = skillId;
            CurrentLevel = currentLevel;
        }

        public void SetCurrentLevel(int currentLevel)
        {
            if (currentLevel <= 0)
                throw new ArgumentException("Current level must be greater than zero.", nameof(currentLevel));

            CurrentLevel = currentLevel;
        }
    }
}
