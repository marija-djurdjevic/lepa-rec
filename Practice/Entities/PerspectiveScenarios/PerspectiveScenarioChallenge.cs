using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class PerspectiveScenarioChallenge : Entity<Guid>, IAggregateRoot
    {
        private readonly List<PerspectiveScenarioQuestion> _questions = new();

        public ChallengeLevel ChallengeLevel { get; private set; }
        public PerspectiveScenarioContext Context { get; private set; }
        public int ActorCount { get; private set; }
        public string ScenarioText { get; private set; } = string.Empty;
        public string? ScenarioTextEn { get; private set; }

        public IReadOnlyCollection<PerspectiveScenarioQuestion> Questions => _questions;

        private PerspectiveScenarioChallenge() : base() { }

        public PerspectiveScenarioChallenge(
            Guid id,
            PerspectiveScenarioContext context,
            int actorCount,
            string scenarioText,
            ChallengeLevel challengeLevel,
            IEnumerable<(Guid Id, Guid SkillId, int Order, string QuestionText, string Reveal)> questions)
            : this(
                  id,
                  context,
                  actorCount,
                  scenarioText,
                  challengeLevel,
                  questions.Select(q => (q.Id, q.SkillId, q.Order, q.QuestionText, q.Reveal, (string?)null, (string?)null)),
                  null)
        {
        }

        public PerspectiveScenarioChallenge(
            Guid id,
            PerspectiveScenarioContext context,
            int actorCount,
            string scenarioText,
            ChallengeLevel challengeLevel,
            IEnumerable<(Guid Id, Guid SkillId, int Order, string QuestionText, string Reveal, string? QuestionTextEn, string? RevealEn)> questions,
            string? scenarioTextEn = null) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (actorCount < 1)
                throw new ArgumentException("Actor count must be at least 1.", nameof(actorCount));

            if (string.IsNullOrWhiteSpace(scenarioText))
                throw new ArgumentException("Scenario text must be provided.", nameof(scenarioText));

            var questionList = questions?.ToList()
                ?? throw new ArgumentNullException(nameof(questions));

            ValidateQuestionCount(challengeLevel, questionList.Count);

            if (questionList.Any(x => x.Id == Guid.Empty))
                throw new ArgumentException("Each question must have a valid id.", nameof(questions));

            if (questionList.Select(x => x.Id).Distinct().Count() != questionList.Count)
                throw new ArgumentException("Question ids must be unique.", nameof(questions));

            if (questionList.Any(x => x.Order < 1))
                throw new ArgumentException("Question order must start at 1 and be positive.", nameof(questions));

            if (questionList.Select(x => x.Order).Distinct().Count() != questionList.Count)
                throw new ArgumentException("Question order must be unique within challenge.", nameof(questions));

            ChallengeLevel = challengeLevel;
            Context = context;
            ActorCount = actorCount;
            ScenarioText = scenarioText.Trim();
            ScenarioTextEn = NormalizeOptional(scenarioTextEn);

            foreach (var question in questionList)
            {
                _questions.Add(new PerspectiveScenarioQuestion(
                    question.Id,
                    id,
                    question.SkillId,
                    question.Order,
                    question.QuestionText,
                    question.Reveal,
                    question.QuestionTextEn,
                    question.RevealEn));
            }
        }

        public PerspectiveScenarioQuestion AddQuestion(Guid questionId, Guid skillId, int order, string questionText, string reveal)
        {
            if (questionId == Guid.Empty)
                throw new ArgumentException("Question id must be provided.", nameof(questionId));

            if (_questions.Any(x => x.Id == questionId))
                throw new InvalidOperationException("Question id already exists in this challenge.");

            if (_questions.Any(x => x.Order == order))
                throw new InvalidOperationException("Question order already exists in this challenge.");

            var question = new PerspectiveScenarioQuestion(
                questionId,
                Id,
                skillId,
                order,
                questionText,
                reveal);

            _questions.Add(question);
            EnsureQuestionCountStillMatchesDifficulty();

            return question;
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }

        private static void ValidateQuestionCount(ChallengeLevel challengeLevel, int questionCount)
        {
            if (questionCount == 0)
                throw new ArgumentException("At least one question is required.", nameof(questionCount));

            if (challengeLevel == ChallengeLevel.Hard && questionCount < 2)
                throw new ArgumentException("Hard perspective scenarios must contain multiple questions.", nameof(questionCount));
        }

        private void EnsureQuestionCountStillMatchesDifficulty()
        {
            ValidateQuestionCount(ChallengeLevel, _questions.Count);
        }
    }
}
