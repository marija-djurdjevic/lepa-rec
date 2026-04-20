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

            if (_answers.Count > 0)
                throw new InvalidOperationException("Answers have already been started. Use per-question flow.");

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

        public void SubmitOrUpdateAnswer(ScenarioAnswer answer)
        {
            if (SubmittedAt.HasValue)
                throw new InvalidOperationException("Answers have already been submitted.");

            ArgumentNullException.ThrowIfNull(answer);

            var existingIndex = _answers.FindIndex(x => x.QuestionId == answer.QuestionId);
            if (existingIndex >= 0)
            {
                _answers[existingIndex] = answer;
                return;
            }

            _answers.Add(answer);
        }

        public void MarkSubmitted(DateTime submittedAt)
        {
            if (SubmittedAt.HasValue)
                return;

            if (_answers.Count == 0)
                throw new InvalidOperationException("At least one answer must be provided.");

            SubmittedAt = submittedAt;
        }

        public bool IsCompleted()
        {
            return SubmittedAt.HasValue;
        }
    }
}
