using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;

namespace AngularNetBase.Practice.Entities.Skills
{
    public class Skill : Entity<Guid>, IAggregateRoot
    {
        private readonly List<SkillLevelDefinition> _levels = new();

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public IReadOnlyCollection<SkillLevelDefinition> Levels => _levels;

        private Skill() : base() { }

        public Skill(Guid id, string name, string description) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Skill name must be provided.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Skill description must be provided.", nameof(description));

            Name = name.Trim();
            Description = description.Trim();
        }

        public SkillLevelDefinition AddLevel(int levelNumber, string title, string description)
        {
            if (_levels.Any(x => x.LevelNumber == levelNumber))
                throw new InvalidOperationException("Skill level number must be unique within a skill.");

            var level = new SkillLevelDefinition(levelNumber, title, description);
            _levels.Add(level);

            return level;
        }
    }
}
