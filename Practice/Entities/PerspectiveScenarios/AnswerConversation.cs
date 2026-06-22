using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class AnswerConversation : Entity<Guid>, IAggregateRoot
    {
        private readonly List<ConversationTurn> _turns = new();

        public Guid UserId { get; private set; }
        public Guid ExerciseId { get; private set; }
        public Guid QuestionId { get; private set; }
        public ConversationStatus Status { get; private set; }
        public int GuideIterationCount { get; private set; }
        public int MaxGuideIterations { get; private set; }
        public IReadOnlyCollection<ConversationTurn> Turns => _turns;

        private AnswerConversation() : base() { }

        public AnswerConversation(
            Guid id,
            Guid userId,
            Guid exerciseId,
            Guid questionId,
            int maxGuideIterations = 3) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be a valid GUID.", nameof(userId));

            if (exerciseId == Guid.Empty)
                throw new ArgumentException("Exercise id must be a valid GUID.", nameof(exerciseId));

            if (questionId == Guid.Empty)
                throw new ArgumentException("Question id must be a valid GUID.", nameof(questionId));

            if (maxGuideIterations < 1)
                throw new ArgumentOutOfRangeException(nameof(maxGuideIterations), "At least one guide iteration is required.");

            UserId = userId;
            ExerciseId = exerciseId;
            QuestionId = questionId;
            Status = ConversationStatus.InProgress;
            MaxGuideIterations = maxGuideIterations;
        }

        public ConversationTurn RegisterLearnerAnswer(
            Guid turnId,
            string answer,
            EvaluationSummary evaluation,
            DateTime createdAt,
            string? idempotencyKey)
        {
            if (IsClosed())
                throw new InvalidOperationException("Conversation is already closed.");

            var normalizedKey = NormalizeOptional(idempotencyKey);
            if (normalizedKey is not null && HasProcessedIdempotencyKey(normalizedKey))
                throw new InvalidOperationException("Idempotency key has already been processed.");

            var turn = ConversationTurn.LearnerAnswer(
                turnId,
                answer,
                evaluation,
                createdAt,
                normalizedKey);

            _turns.Add(turn);
            return turn;
        }

        public ConversationTurn RegisterGuideQuestion(
            Guid turnId,
            string question,
            string? whyThisQuestion,
            DateTime createdAt)
        {
            if (IsClosed())
                throw new InvalidOperationException("Conversation is already closed.");

            if (!CanAskAnotherGuideQuestion())
                throw new InvalidOperationException("Maximum guide iterations have been reached.");

            var turn = ConversationTurn.GuideQuestion(
                turnId,
                question,
                whyThisQuestion,
                createdAt);

            _turns.Add(turn);
            GuideIterationCount++;
            return turn;
        }

        public bool CanAskAnotherGuideQuestion()
        {
            return !IsClosed() && GuideIterationCount < MaxGuideIterations;
        }

        public void MarkCompleted()
        {
            Status = ConversationStatus.Completed;
        }

        public void MarkMaxIterationsReached()
        {
            Status = ConversationStatus.MaxIterationsReached;
        }

        public bool IsClosed()
        {
            return Status is ConversationStatus.Completed or ConversationStatus.MaxIterationsReached;
        }

        public bool HasProcessedIdempotencyKey(string? idempotencyKey)
        {
            var normalized = NormalizeOptional(idempotencyKey);
            return normalized is not null
                && _turns.Any(x => x.IdempotencyKey == normalized);
        }

        public ConversationTurn? LastGuideTurn()
        {
            return _turns
                .Where(x => x.Role == ConversationTurnRole.Guide)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();
        }

        public ConversationTurn? LastLearnerTurn()
        {
            return _turns
                .Where(x => x.Role == ConversationTurnRole.Learner)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
