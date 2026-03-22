using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class PerspectiveScenarioChallenge : Entity<Guid>, IAggregateRoot
    {
        private readonly List<PerspectiveScenarioQuestion> _questions = new();

        public ChallengeLevel ChallengeLevel { get; private set; }
        public string ScenarioText { get; private set; } = string.Empty;
        public string Reveal { get; private set; } = string.Empty;

        public IReadOnlyCollection<PerspectiveScenarioQuestion> Questions => _questions;

        private PerspectiveScenarioChallenge() : base() { }

        public PerspectiveScenarioChallenge(
            Guid id,
            string scenarioText,
            string reveal,
            ChallengeLevel challengeLevel,
            IEnumerable<(Guid Id, Guid SkillId, string QuestionText)> questions) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.", nameof(id));

            if (string.IsNullOrWhiteSpace(scenarioText))
                throw new ArgumentException("Scenario text must be provided.", nameof(scenarioText));

            if (string.IsNullOrWhiteSpace(reveal))
                throw new ArgumentException("Reveal must be provided.", nameof(reveal));

            var questionList = questions?.ToList()
                ?? throw new ArgumentNullException(nameof(questions));

            ValidateQuestionCount(challengeLevel, questionList.Count);

            if (questionList.Any(x => x.Id == Guid.Empty))
                throw new ArgumentException("Each question must have a valid id.", nameof(questions));

            if (questionList.Select(x => x.Id).Distinct().Count() != questionList.Count)
                throw new ArgumentException("Question ids must be unique.", nameof(questions));

            ChallengeLevel = challengeLevel;
            ScenarioText = scenarioText.Trim();
            Reveal = reveal.Trim();

            foreach (var question in questionList)
            {
                _questions.Add(new PerspectiveScenarioQuestion(
                    question.Id,
                    id,
                    question.SkillId,
                    question.QuestionText));
            }
        }

        public PerspectiveScenarioQuestion AddQuestion(Guid questionId, Guid skillId, string questionText)
        {
            if (questionId == Guid.Empty)
                throw new ArgumentException("Question id must be provided.", nameof(questionId));

            if (_questions.Any(x => x.Id == questionId))
                throw new InvalidOperationException("Question id already exists in this challenge.");

            var question = new PerspectiveScenarioQuestion(
                questionId,
                Id,
                skillId,
                questionText);

            _questions.Add(question);
            EnsureQuestionCountStillMatchesDifficulty();

            return question;
        }

        private static void ValidateQuestionCount(ChallengeLevel challengeLevel, int questionCount)
        {
            if (questionCount == 0)
                throw new ArgumentException("At least one question is required.", nameof(questionCount));

            if (challengeLevel == ChallengeLevel.Easy && questionCount != 1)
                throw new ArgumentException("Easy perspective scenarios must contain exactly one question.", nameof(questionCount));

            if (challengeLevel == ChallengeLevel.Hard && questionCount < 2)
                throw new ArgumentException("Hard perspective scenarios must contain multiple questions.", nameof(questionCount));
        }

        private void EnsureQuestionCountStillMatchesDifficulty()
        {
            ValidateQuestionCount(ChallengeLevel, _questions.Count);
        }
    }
}
