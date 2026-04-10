namespace AngularNetBase.Practice.Entities.Skills
{
    public class SkillLevelDefinition
    {
        public int LevelNumber { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        private SkillLevelDefinition() { }

        public SkillLevelDefinition(int levelNumber, string title, string description)
        {
            if (levelNumber <= 0)
                throw new ArgumentException("Level number must be greater than zero.", nameof(levelNumber));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Level title must be provided.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Level description must be provided.", nameof(description));

            LevelNumber = levelNumber;
            Title = title.Trim();
            Description = description.Trim();
        }
    }
}
