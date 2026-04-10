using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class PerspectiveScenarioExercise : Entity<Guid>, IAggregateRoot
    {
        private readonly List<ScenarioAnswer> _answers = new();

        public Guid UserId { get; private set; }
        public Guid ChallengeId { get; private set; }
        public DateTime? SubmittedAt { get; private set; }

        public IReadOnlyCollection<ScenarioAnswer> Answers => _answers;

        private PerspectiveScenarioExercise() : base() { }

        public PerspectiveScenarioExercise(Guid id, Guid userId, Guid challengeId) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be a valid GUID.", nameof(userId));

            if (challengeId == Guid.Empty)
                throw new ArgumentException("Challenge id must be a valid GUID.", nameof(challengeId));

            UserId = userId;
            ChallengeId = challengeId;
        }

        public void SubmitAnswers(IEnumerable<ScenarioAnswer> answers, DateTime submittedAt)
        {
            if (SubmittedAt.HasValue)
                throw new InvalidOperationException("Answers have already been submitted.");

            var answerList = answers?.ToList()
                ?? throw new ArgumentNullException(nameof(answers));

            if (answerList.Count == 0)
                throw new ArgumentException("At least one answer must be provided.", nameof(answers));

            if (answerList.Any(x => x.QuestionId == Guid.Empty))
                throw new ArgumentException("Each answer must reference a valid question id.", nameof(answers));

            if (answerList.Select(x => x.QuestionId).Distinct().Count() != answerList.Count)
                throw new ArgumentException("Only one answer per question is allowed.", nameof(answers));

            _answers.AddRange(answerList);
            SubmittedAt = submittedAt;
        }

        public bool IsCompleted()
        {
            return SubmittedAt.HasValue;
        }
    }
}
