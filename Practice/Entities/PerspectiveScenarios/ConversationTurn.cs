using AngularNetBase.Shared.Core.Domain;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class ConversationTurn : Entity<Guid>
    {
        public ConversationTurnRole Role { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public EvaluationSummary? EvaluationSummary { get; private set; }
        public string? WhyThisQuestion { get; private set; }
        public string? IdempotencyKey { get; private set; }

        private ConversationTurn() : base() { }

        private ConversationTurn(
            Guid id,
            ConversationTurnRole role,
            string message,
            DateTime createdAt,
            EvaluationSummary? evaluationSummary,
            string? whyThisQuestion,
            string? idempotencyKey) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message must be provided.", nameof(message));

            Role = role;
            Message = message.Trim();
            CreatedAt = createdAt;
            EvaluationSummary = evaluationSummary;
            WhyThisQuestion = NormalizeOptional(whyThisQuestion);
            IdempotencyKey = NormalizeOptional(idempotencyKey);
        }

        public static ConversationTurn LearnerAnswer(
            Guid id,
            string answer,
            EvaluationSummary evaluationSummary,
            DateTime createdAt,
            string? idempotencyKey)
        {
            ArgumentNullException.ThrowIfNull(evaluationSummary);

            return new ConversationTurn(
                id,
                ConversationTurnRole.Learner,
                answer,
                createdAt,
                evaluationSummary,
                null,
                idempotencyKey);
        }

        public static ConversationTurn GuideQuestion(
            Guid id,
            string question,
            string? whyThisQuestion,
            DateTime createdAt)
        {
            return new ConversationTurn(
                id,
                ConversationTurnRole.Guide,
                question,
                createdAt,
                null,
                whyThisQuestion,
                null);
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
