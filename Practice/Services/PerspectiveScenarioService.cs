using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Entities.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class PerspectiveScenarioService : IPerspectiveScenarioService
    {
        private readonly IPerspectiveScenarioChallengeRepository _challengeRepository;
        private readonly IPerspectiveScenarioExerciseRepository _exerciseRepository;
        private readonly ISessionRepository _dailySessionRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PerspectiveScenarioService(
            IPerspectiveScenarioChallengeRepository challengeRepository,
            IPerspectiveScenarioExerciseRepository exerciseRepository,
            ISessionRepository dailySessionRepository,
            ISkillRepository skillRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _challengeRepository = challengeRepository;
            _exerciseRepository = exerciseRepository;
            _dailySessionRepository = dailySessionRepository;
            _skillRepository = skillRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<PerspectiveScenarioChallengeDto> CreateChallengeAsync(
            CreatePerspectiveScenarioChallengeDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.Questions is null || !dto.Questions.Any())
                throw new ArgumentException("At least one question must be provided.", nameof(dto));

            var skillIds = dto.Questions.Select(x => x.SkillId).Distinct().ToList();
            var skills = await _skillRepository.GetByIdsAsync(skillIds, cancellationToken);

            if (skills.Count != skillIds.Count)
                throw new InvalidOperationException("One or more referenced skills were not found.");

            var challenge = new PerspectiveScenarioChallenge(
                Guid.NewGuid(),
                dto.ScenarioText,
                dto.Reveal,
                dto.ChallengeLevel,
                dto.Questions.Select(x => (Guid.NewGuid(), x.SkillId, x.QuestionText)));

            await _challengeRepository.AddAsync(challenge, cancellationToken);
            await _challengeRepository.SaveChangesAsync(cancellationToken);

            return MapChallenge(challenge);
        }

        public async Task<IEnumerable<PerspectiveScenarioChallengeDto>> GetAllChallengesAsync(
            CancellationToken cancellationToken = default)
        {
            var challenges = await _challengeRepository.GetAllAsync(cancellationToken);
            return challenges.Select(MapChallenge);
        }

        public async Task<IEnumerable<PerspectiveScenarioChallengeDto>> GetChallengesByLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default)
        {
            var challenges = await _challengeRepository.GetByChallengeLevelAsync(challengeLevel, cancellationToken);
            return challenges.Select(MapChallenge);
        }

        public async Task<PerspectiveScenarioPromptDto> GetRandomChallengeAsync(
            ChallengeLevel level,
            CancellationToken cancellationToken = default)
        {
            var challenge = await _challengeRepository.GetRandomByLevelAsync(level, cancellationToken);

            if (challenge is null)
                throw new InvalidOperationException("No perspective scenarios found for the selected level.");

            return MapPrompt(challenge);
        }

        public async Task<PerspectiveScenarioExerciseDto> StartExerciseAsync(
            StartPerspectiveScenarioExerciseDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.UserId == Guid.Empty)
                throw new ArgumentException("UserId must be provided.", nameof(dto));

            if (dto.ChallengeId == Guid.Empty)
                throw new ArgumentException("ChallengeId must be provided.", nameof(dto));

            var challenge = await _challengeRepository.GetByIdAsync(dto.ChallengeId, cancellationToken);
            if (challenge is null)
                throw new InvalidOperationException("Perspective scenario challenge was not found.");

            var exercise = new PerspectiveScenarioExercise(
                Guid.NewGuid(),
                dto.UserId,
                dto.ChallengeId);

            await _exerciseRepository.AddAsync(exercise, cancellationToken);
            await _exerciseRepository.SaveChangesAsync(cancellationToken);

            return MapExercise(exercise);
        }

        public async Task<PerspectiveScenarioExerciseDto?> GetExerciseByIdAsync(
            Guid userId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Exercise id must be provided.", nameof(id));

            var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
            if (exercise is null || exercise.UserId != userId)
                return null;

            return exercise is null ? null : MapExercise(exercise);
        }

        public async Task<IEnumerable<PerspectiveScenarioExerciseDto>> GetExercisesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.", nameof(userId));

            var exercises = await _exerciseRepository.GetByUserIdAsync(userId, cancellationToken);
            return exercises.Select(MapExercise);
        }

        public async Task<SubmitPerspectiveScenarioResultDto> SubmitAnswersAsync(
            Guid userId,
            SubmitPerspectiveScenarioAnswerDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.", nameof(dto));

            if (dto.Answers is null || !dto.Answers.Any())
                throw new ArgumentException("At least one answer must be provided.", nameof(dto));

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Perspective scenario exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var challenge = await _challengeRepository.GetByIdAsync(exercise.ChallengeId, cancellationToken);
            if (challenge is null)
                throw new InvalidOperationException("Perspective scenario challenge was not found.");

            var dailySession = await GetOrCreateTodaySessionAsync(exercise.UserId, cancellationToken);

            EnsureAnswersMatchChallengeQuestions(challenge, dto.Answers);

            var submittedAt = _dateTimeProvider.UtcNow;
            var answers = dto.Answers
                .Select(x => new ScenarioAnswer(x.QuestionId, x.AnswerText))
                .ToList();

            exercise.SubmitAnswers(answers, submittedAt);

            dailySession.RecordExercise(
                exercise.Id,
                ExerciseType.PerspectiveScenario,
                submittedAt);

            await _dailySessionRepository.UpdateAsync(dailySession, cancellationToken);
            await _dailySessionRepository.SaveChangesAsync(cancellationToken);

            return new SubmitPerspectiveScenarioResultDto(
                MapExercise(exercise),
                challenge.Reveal);
        }

        private static void EnsureAnswersMatchChallengeQuestions(
            PerspectiveScenarioChallenge challenge,
            IReadOnlyCollection<SubmitPerspectiveScenarioAnswerItemDto> answers)
        {
            var expectedQuestionIds = challenge.Questions
                .Select(x => x.Id)
                .OrderBy(x => x)
                .ToList();

            var submittedQuestionIds = answers
                .Select(x => x.QuestionId)
                .OrderBy(x => x)
                .ToList();

            if (expectedQuestionIds.Count != submittedQuestionIds.Count)
                throw new InvalidOperationException("All scenario questions must be answered exactly once.");

            if (!expectedQuestionIds.SequenceEqual(submittedQuestionIds))
                throw new InvalidOperationException("Submitted answers do not match the scenario questions.");
        }

        private static PerspectiveScenarioChallengeDto MapChallenge(PerspectiveScenarioChallenge challenge)
        {
            return new PerspectiveScenarioChallengeDto(
                challenge.Id,
                challenge.ScenarioText,
                challenge.Reveal,
                challenge.ChallengeLevel,
                challenge.Questions.Select(MapQuestion).ToList());
        }

        private static PerspectiveScenarioPromptDto MapPrompt(PerspectiveScenarioChallenge challenge)
        {
            return new PerspectiveScenarioPromptDto(
                challenge.Id,
                challenge.ScenarioText,
                challenge.ChallengeLevel,
                challenge.Questions.Select(MapQuestion).ToList());
        }

        private static PerspectiveScenarioQuestionDto MapQuestion(PerspectiveScenarioQuestion question)
        {
            return new PerspectiveScenarioQuestionDto(
                question.Id,
                question.SkillId,
                question.QuestionText);
        }

        private static PerspectiveScenarioExerciseDto MapExercise(PerspectiveScenarioExercise exercise)
        {
            return new PerspectiveScenarioExerciseDto(
                exercise.Id,
                exercise.UserId,
                exercise.ChallengeId,
                exercise.Answers.Select(x => new ScenarioAnswerDto(x.QuestionId, x.AnswerText)).ToList(),
                exercise.SubmittedAt,
                exercise.IsCompleted());
        }

        private async Task<DailySession> GetOrCreateTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var today = _dateTimeProvider.UtcNow.Date;

            var session = await _dailySessionRepository.GetByUserAndDateAsync(
                userId,
                today,
                cancellationToken);

            if (session is not null)
                return session;

            session = new DailySession(Guid.NewGuid(), userId, today);
            await _dailySessionRepository.AddAsync(session, cancellationToken);
            await _dailySessionRepository.SaveChangesAsync(cancellationToken);

            return session;
        }
    }
}
