namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class ScenarioAnswer
    {
        public Guid QuestionId { get; private set; }
        public string AnswerText { get; private set; } = string.Empty;

        private ScenarioAnswer() { }

        public ScenarioAnswer(Guid questionId, string answerText)
        {
            if (questionId == Guid.Empty)
                throw new ArgumentException("Question id must be provided.", nameof(questionId));

            if (string.IsNullOrWhiteSpace(answerText))
                throw new ArgumentException("Answer text must be provided.", nameof(answerText));

            QuestionId = questionId;
            AnswerText = answerText.Trim();
        }
    }
}
