using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Dtos.Sessions;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Practice.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IDistancedJournalExerciseRepository _distancedJournalExerciseRepository;
        private readonly IDistancedJournalChallengeRepository _distancedJournalChallengeRepository;
        private readonly IPerspectiveScenarioChallengeRepository _perspectiveScenarioChallengeRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SessionService(
            ISessionRepository sessionRepository,
            IDistancedJournalExerciseRepository distancedJournalExerciseRepository,
            IDistancedJournalChallengeRepository distancedJournalChallengeRepository,
            IPerspectiveScenarioChallengeRepository perspectiveScenarioChallengeRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _sessionRepository = sessionRepository;
            _distancedJournalExerciseRepository = distancedJournalExerciseRepository;
            _distancedJournalChallengeRepository = distancedJournalChallengeRepository;
            _perspectiveScenarioChallengeRepository = perspectiveScenarioChallengeRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<DailySessionStateDto> GetOrCreateTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            return MapToStateDto(session);
        }

        public async Task<DailySessionStateDto> CompletePrimerAsync(
            Guid userId,
            CompletePrimerDto dto,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            var now = _dateTimeProvider.UtcNow;

            if (dto.IsSkipped)
            {
                if (dto.SelectedStatementId.HasValue || dto.GrowthMessageId.HasValue)
                    throw new InvalidOperationException("Ako je primer preskocen, SelectedStatementId i GrowthMessageId moraju biti null.");

                session.SkipPrimer(now);
            }
            else
            {
                if (dto.PresentedStatementIds == null || !dto.PresentedStatementIds.Any())
                    throw new InvalidOperationException("Mora postojati barem jedna ponudena izjava.");

                if (!dto.SelectedStatementId.HasValue)
                    throw new InvalidOperationException("SelectedStatementId je obavezan kada primer nije preskocen.");

                if (!dto.GrowthMessageId.HasValue)
                    throw new InvalidOperationException("GrowthMessageId je obavezan kada primer nije preskocen.");

                session.CompletePrimer(
                    dto.PresentedStatementIds,
                    dto.SelectedStatementId.Value,
                    dto.GrowthMessageId.Value,
                    now);
            }

            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return MapToStateDto(session);
        }

        public async Task<DailySessionStateDto> RecordExerciseAsync(
            Guid userId,
            RecordExerciseDto dto,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            var now = _dateTimeProvider.UtcNow;

            session.RecordExercise(dto.ExerciseId, dto.Type, now);

            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return MapToStateDto(session);
        }

        public async Task<DailySessionStateDto> CompleteTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            var now = _dateTimeProvider.UtcNow;

            session.Complete(now);

            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return MapToStateDto(session);
        }

        public async Task<TodayPracticePlanDto> GetTodayPracticePlanAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId must be provided.");

            var today = _dateTimeProvider.UtcNow.Date;

            var todaySession = await _sessionRepository.GetByUserAndDateAsync(
                userId,
                today,
                cancellationToken);

            var todayExerciseRecords = todaySession?.Events
                .OfType<ExerciseRecord>()
                .OrderBy(e => e.Timestamp)
                .ToList() ?? new List<ExerciseRecord>();

            var hasDistancedJournalToday = todayExerciseRecords.Any(e => e.Type == ExerciseType.DistancedJournal);
            var hasDistancedJournalReflectionToday = todayExerciseRecords.Any(e => e.Type == ExerciseType.DistancedJournalReflection);
            var hasPerspectiveScenarioToday = todayExerciseRecords.Any(e => e.Type == ExerciseType.PerspectiveScenario);

            var yesterday = today.AddDays(-1);

            var yesterdaySession = await _sessionRepository.GetByUserAndDateAsync(
                userId,
                yesterday,
                cancellationToken);

            if (yesterdaySession is null || !yesterdaySession.HasRecordedExercises)
            {
                if (hasDistancedJournalToday)
                {
                    return new TodayPracticePlanDto(
                        null,
                        Array.Empty<DistancedJournalChallengeDto>(),
                        null,
                        false,
                        true,
                        false,
                        false);
                }

                return new TodayPracticePlanDto(
                    null,
                    await BuildDistancedJournalChoicesAsync(cancellationToken),
                    null,
                    false,
                    false,
                    false,
                    false);
            }

            var yesterdayExerciseRecords = yesterdaySession.Events
                .OfType<ExerciseRecord>()
                .OrderBy(e => e.Timestamp)
                .ToList();

            var hadPerspectiveScenarioYesterday = yesterdayExerciseRecords.Any(e => e.Type == ExerciseType.PerspectiveScenario);
            var hadDistancedJournalYesterday = yesterdayExerciseRecords.Any(e => e.Type == ExerciseType.DistancedJournal);
            var hadDistancedJournalReflectionYesterday = yesterdayExerciseRecords.Any(e => e.Type == ExerciseType.DistancedJournalReflection);

            if (hadPerspectiveScenarioYesterday)
            {
                if (hasDistancedJournalToday)
                {
                    return new TodayPracticePlanDto(
                        null,
                        Array.Empty<DistancedJournalChallengeDto>(),
                        null,
                        false,
                        true,
                        false,
                        false);
                }

                return new TodayPracticePlanDto(
                    null,
                    await BuildDistancedJournalChoicesAsync(cancellationToken),
                    null,
                    false,
                    false,
                    false,
                    false);
            }

            if (hadDistancedJournalYesterday)
            {
                DistancedJournalReflectionPromptDto? reflectionPrompt = null;

                if (!hasDistancedJournalReflectionToday)
                {
                    reflectionPrompt = await BuildReflectionPromptFromYesterdayAsync(
                        yesterdayExerciseRecords,
                        cancellationToken);
                }

                var perspectiveScenarioPrompt = !hasPerspectiveScenarioToday
                    ? await BuildPerspectiveScenarioPromptAsync(cancellationToken)
                    : null;

                return new TodayPracticePlanDto(
                    reflectionPrompt,
                    Array.Empty<DistancedJournalChallengeDto>(),
                    perspectiveScenarioPrompt,
                    !hasPerspectiveScenarioToday,
                    false,
                    hasDistancedJournalReflectionToday,
                    hasPerspectiveScenarioToday);
            }

            if (hadDistancedJournalReflectionYesterday)
            {
                var perspectiveScenarioPrompt = !hasPerspectiveScenarioToday
                    ? await BuildPerspectiveScenarioPromptAsync(cancellationToken)
                    : null;

                return new TodayPracticePlanDto(
                    null,
                    Array.Empty<DistancedJournalChallengeDto>(),
                    perspectiveScenarioPrompt,
                    !hasPerspectiveScenarioToday,
                    false,
                    hasDistancedJournalReflectionToday,
                    hasPerspectiveScenarioToday);
            }

            if (hasDistancedJournalToday)
            {
                return new TodayPracticePlanDto(
                    null,
                    Array.Empty<DistancedJournalChallengeDto>(),
                    null,
                    false,
                    true,
                    false,
                    false);
            }

            return new TodayPracticePlanDto(
                null,
                await BuildDistancedJournalChoicesAsync(cancellationToken),
                null,
                false,
                false,
                false,
                false);
        }

        private async Task<DailySession> GetOrCreateTodaySessionEntityAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var today = _dateTimeProvider.UtcNow.Date;

            var session = await _sessionRepository.GetByUserAndDateAsync(
                userId,
                today,
                cancellationToken);

            if (session != null)
                return session;

            session = new DailySession(Guid.NewGuid(), userId, today);

            await _sessionRepository.AddAsync(session, cancellationToken);
            await _sessionRepository.SaveChangesAsync(cancellationToken);

            return session;
        }

        private async Task<DistancedJournalReflectionPromptDto?> BuildReflectionPromptFromYesterdayAsync(
            List<ExerciseRecord> exerciseRecords,
            CancellationToken cancellationToken)
        {
            var lastDistancedJournalRecord = exerciseRecords
                .Where(e => e.Type == ExerciseType.DistancedJournal)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefault();

            if (lastDistancedJournalRecord is null)
                return null;

            var exercise = await _distancedJournalExerciseRepository.GetByIdAsync(
                lastDistancedJournalRecord.ExerciseId,
                cancellationToken);

            if (exercise is null || exercise.Answer is null)
                return null;

            var challenge = await _distancedJournalChallengeRepository.GetByIdAsync(
                exercise.ChallengeId,
                cancellationToken);

            if (challenge is null)
                return null;

            return new DistancedJournalReflectionPromptDto(
                exercise.Id,
                challenge.Content,
                challenge.FollowUpQuestion,
                exercise.Answer.MainAnswer,
                exercise.Answer.FollowUpAnswer);
        }

        private async Task<IReadOnlyCollection<DistancedJournalChallengeDto>> BuildDistancedJournalChoicesAsync(
            CancellationToken cancellationToken)
        {
            var levelPairs = new List<(ChallengeLevel First, ChallengeLevel Second)>
            {
                (ChallengeLevel.Easy, ChallengeLevel.Medium),
                (ChallengeLevel.Easy, ChallengeLevel.Hard),
                (ChallengeLevel.Medium, ChallengeLevel.Hard)
            };

            var selectedPair = levelPairs[Random.Shared.Next(levelPairs.Count)];

            var firstChallenge = await _distancedJournalChallengeRepository.GetRandomByLevelAsync(
                selectedPair.First,
                cancellationToken);

            var secondChallenge = await _distancedJournalChallengeRepository.GetRandomByLevelAsync(
                selectedPair.Second,
                cancellationToken);

            var results = new List<DistancedJournalChallengeDto>();

            if (firstChallenge is not null)
            {
                results.Add(new DistancedJournalChallengeDto(
                    firstChallenge.Id,
                    firstChallenge.Content,
                    firstChallenge.FollowUpQuestion,
                    firstChallenge.ChallengeLevel));
            }

            if (secondChallenge is not null)
            {
                results.Add(new DistancedJournalChallengeDto(
                    secondChallenge.Id,
                    secondChallenge.Content,
                    secondChallenge.FollowUpQuestion,
                    secondChallenge.ChallengeLevel));
            }

            return results;
        }

        private async Task<PerspectiveScenarioPromptDto?> BuildPerspectiveScenarioPromptAsync(
            CancellationToken cancellationToken)
        {
            var levels = new[] { ChallengeLevel.Easy, ChallengeLevel.Medium, ChallengeLevel.Hard }
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            foreach (var level in levels)
            {
                var challenge = await _perspectiveScenarioChallengeRepository.GetRandomByLevelAsync(
                    level,
                    cancellationToken);

                if (challenge is null)
                    continue;

                return new PerspectiveScenarioPromptDto(
                    challenge.Id,
                    challenge.ScenarioText,
                    challenge.ChallengeLevel,
                    challenge.Questions
                        .Select(q => new PerspectiveScenarioQuestionDto(
                            q.Id,
                            q.SkillId,
                            q.QuestionText))
                        .ToList());
            }

            return null;
        }

        private static DailySessionStateDto MapToStateDto(DailySession session)
        {
            return new DailySessionStateDto(
                session.Id,
                session.Status.ToString(),
                session.RequiresPrimer,
                session.PrimerCompleted,
                session.PrimerSkipped,
                session.CompletedExercisesCount);
        }
    }
}
