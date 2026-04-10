using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Dtos.Sessions;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AngularNetBase.Practice.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IDistancedJournalExerciseRepository _distancedJournalExerciseRepository;
        private readonly IDistancedJournalChallengeRepository _distancedJournalChallengeRepository;
        private readonly IPerspectiveScenarioChallengeRepository _perspectiveScenarioChallengeRepository;
        private readonly IPerspectiveScenarioExerciseRepository _perspectiveScenarioExerciseRepository;
        private readonly IDailyChallengeAssignmentService _dailyChallengeAssignmentService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SessionService(
            ISessionRepository sessionRepository,
            IDistancedJournalExerciseRepository distancedJournalExerciseRepository,
            IDistancedJournalChallengeRepository distancedJournalChallengeRepository,
            IPerspectiveScenarioChallengeRepository perspectiveScenarioChallengeRepository,
            IPerspectiveScenarioExerciseRepository perspectiveScenarioExerciseRepository,
            IDailyChallengeAssignmentService dailyChallengeAssignmentService,
            IDateTimeProvider dateTimeProvider)
        {
            _sessionRepository = sessionRepository;
            _distancedJournalExerciseRepository = distancedJournalExerciseRepository;
            _distancedJournalChallengeRepository = distancedJournalChallengeRepository;
            _perspectiveScenarioChallengeRepository = perspectiveScenarioChallengeRepository;
            _perspectiveScenarioExerciseRepository = perspectiveScenarioExerciseRepository;
            _dailyChallengeAssignmentService = dailyChallengeAssignmentService;
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
                session.CompletePrimer(
                    dto.PresentedStatementIds ?? new List<Guid>(),
                    dto.SelectedStatementId.GetValueOrDefault(),
                    dto.GrowthMessageId.GetValueOrDefault(),
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

            await ValidateExerciseForRecordingAsync(userId, dto, cancellationToken);

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
            var assignment = await _dailyChallengeAssignmentService.GetOrCreateTodayAssignmentAsync(cancellationToken);

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
                return await BuildPlanBasedOnDistancedJournalTodayAsync(
                    hasDistancedJournalToday,
                    assignment,
                    cancellationToken);
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
                return await BuildPlanBasedOnDistancedJournalTodayAsync(
                    hasDistancedJournalToday,
                    assignment,
                    cancellationToken);
            }

            if (hadDistancedJournalYesterday)
            {
                return await BuildPlanAfterDistancedJournalYesterdayAsync(
                    yesterdayExerciseRecords,
                    hasDistancedJournalReflectionToday,
                    hasPerspectiveScenarioToday,
                    assignment,
                    cancellationToken);
            }

            if (hadDistancedJournalReflectionYesterday)
            {
                return await BuildPlanBasedOnDistancedJournalTodayAsync(
                    hasDistancedJournalToday,
                    assignment,
                    cancellationToken);
            }

            if (hasDistancedJournalToday)
            {
                return BuildPlanWithDistancedJournalCompletedOnly();
            }

            return await BuildPlanWithDistancedJournalChoicesAsync(assignment, cancellationToken);
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

        private async Task ValidateExerciseForRecordingAsync(
            Guid userId,
            RecordExerciseDto dto,
            CancellationToken cancellationToken)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.", nameof(dto));

            if (dto.Type == ExerciseType.PerspectiveScenario)
            {
                var exercise = await _perspectiveScenarioExerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
                if (exercise is null || exercise.UserId != userId)
                    throw new InvalidOperationException("Perspective scenario exercise was not found.");
                return;
            }

            if (dto.Type == ExerciseType.DistancedJournal || dto.Type == ExerciseType.DistancedJournalReflection)
            {
                var exercise = await _distancedJournalExerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
                if (exercise is null || exercise.UserId != userId)
                    throw new InvalidOperationException("Distanced journal exercise was not found.");
                return;
            }

            throw new InvalidOperationException("Unsupported exercise type.");
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

        private async Task<IReadOnlyCollection<DistancedJournalChallengeDto>> BuildDistancedJournalChoicesForTodayAsync(
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            var choices = new List<DistancedJournalChallengeDto>();
            var ids = new[]
            {
                assignment.DistancedJournalChallengeId,
                assignment.DistancedJournalChallengeId2
            };

            foreach (var id in ids.Distinct())
            {
                if (id == Guid.Empty)
                    continue;

                var challenge = await _distancedJournalChallengeRepository.GetByIdAsync(id, cancellationToken);
                if (challenge is null)
                    continue;

                choices.Add(new DistancedJournalChallengeDto(
                    challenge.Id,
                    challenge.Content,
                    challenge.FollowUpQuestion,
                    challenge.ChallengeLevel));
            }

            if (choices.Count < 2)
            {
                var fallback = await BuildFallbackDistancedJournalChoicesAsync(
                    choices.Select(x => x.Id).ToHashSet(),
                    cancellationToken);

                choices.AddRange(fallback);
            }

            return choices.Take(2).ToList();
        }

        private async Task<IReadOnlyCollection<DistancedJournalChallengeDto>> BuildFallbackDistancedJournalChoicesAsync(
            HashSet<Guid> excludeIds,
            CancellationToken cancellationToken)
        {
            var challenges = await _distancedJournalChallengeRepository.GetAllAsync(cancellationToken);
            var available = challenges
                .Where(x => !excludeIds.Contains(x.Id))
                .ToList();

            if (available.Count == 0)
                return Array.Empty<DistancedJournalChallengeDto>();

            var first = available[Random.Shared.Next(available.Count)];
            var results = new List<DistancedJournalChallengeDto>
            {
                new DistancedJournalChallengeDto(
                    first.Id,
                    first.Content,
                    first.FollowUpQuestion,
                    first.ChallengeLevel)
            };

            var remaining = available.Where(x => x.Id != first.Id).ToList();
            if (remaining.Count > 0)
            {
                var second = remaining[Random.Shared.Next(remaining.Count)];
                results.Add(new DistancedJournalChallengeDto(
                    second.Id,
                    second.Content,
                    second.FollowUpQuestion,
                    second.ChallengeLevel));
            }

            return results;
        }

        private async Task<IReadOnlyCollection<PerspectiveScenarioPromptDto>> BuildPerspectiveScenarioChoicesForTodayAsync(
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            var choices = new List<PerspectiveScenarioPromptDto>();
            var ids = new[]
            {
                assignment.PerspectiveScenarioChallengeId,
                assignment.PerspectiveScenarioChallengeId2
            };

            foreach (var id in ids.Distinct())
            {
                if (id == Guid.Empty)
                    continue;

                var challenge = await _perspectiveScenarioChallengeRepository.GetByIdAsync(id, cancellationToken);
                if (challenge is null)
                    continue;

                choices.Add(MapPerspectiveScenarioPrompt(challenge));
            }

            if (choices.Count < 2)
            {
                var fallback = await BuildPerspectiveScenarioChoicesFallbackAsync(
                    choices.Select(x => x.Id).ToHashSet(),
                    cancellationToken);

                choices.AddRange(fallback);
            }

            return choices.Take(2).ToList();
        }

        private async Task<IReadOnlyCollection<PerspectiveScenarioPromptDto>> BuildPerspectiveScenarioChoicesFallbackAsync(
            HashSet<Guid> excludeIds,
            CancellationToken cancellationToken)
        {
            var challenges = await _perspectiveScenarioChallengeRepository.GetAllAsync(cancellationToken);
            var available = challenges
                .Where(x => !excludeIds.Contains(x.Id))
                .ToList();

            if (available.Count == 0)
                return Array.Empty<PerspectiveScenarioPromptDto>();

            var first = available[Random.Shared.Next(available.Count)];
            var results = new List<PerspectiveScenarioPromptDto>
            {
                MapPerspectiveScenarioPrompt(first)
            };

            var remaining = available.Where(x => x.Id != first.Id).ToList();
            if (remaining.Count > 0)
            {
                var second = remaining[Random.Shared.Next(remaining.Count)];
                results.Add(MapPerspectiveScenarioPrompt(second));
            }

            return results;
        }

        private static PerspectiveScenarioPromptDto MapPerspectiveScenarioPrompt(PerspectiveScenarioChallenge challenge)
        {
            return new PerspectiveScenarioPromptDto(
                challenge.Id,
                challenge.ActorCount,
                challenge.Context,
                challenge.ScenarioText,
                challenge.ChallengeLevel,
                challenge.Questions
                    .Select(q => new PerspectiveScenarioQuestionDto(
                        q.Id,
                        q.SkillId,
                        q.QuestionText))
                    .ToList());
        }

        private static TodayPracticePlanDto BuildPlan(
            DistancedJournalReflectionPromptDto? reflectionPrompt,
            IReadOnlyCollection<DistancedJournalChallengeDto> distancedJournalChoices,
            IReadOnlyCollection<PerspectiveScenarioPromptDto> perspectiveScenarioChoices,
            bool shouldShowPerspectiveScenario,
            bool isDistancedJournalCompleted,
            bool isReflectionCompleted,
            bool isPerspectiveScenarioCompleted)
        {
            return new TodayPracticePlanDto(
                reflectionPrompt,
                distancedJournalChoices,
                perspectiveScenarioChoices,
                shouldShowPerspectiveScenario,
                isDistancedJournalCompleted,
                isReflectionCompleted,
                isPerspectiveScenarioCompleted);
        }

        private static TodayPracticePlanDto BuildPlanWithDistancedJournalCompletedOnly()
        {
            return BuildPlan(
                null,
                Array.Empty<DistancedJournalChallengeDto>(),
                Array.Empty<PerspectiveScenarioPromptDto>(),
                false,
                true,
                false,
                false);
        }

        private async Task<TodayPracticePlanDto> BuildPlanWithDistancedJournalChoicesAsync(
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            return BuildPlan(
                null,
                await BuildDistancedJournalChoicesForTodayAsync(assignment, cancellationToken),
                Array.Empty<PerspectiveScenarioPromptDto>(),
                false,
                false,
                false,
                false);
        }

        private async Task<TodayPracticePlanDto> BuildPlanBasedOnDistancedJournalTodayAsync(
            bool hasDistancedJournalToday,
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            if (hasDistancedJournalToday)
                return BuildPlanWithDistancedJournalCompletedOnly();

            return await BuildPlanWithDistancedJournalChoicesAsync(assignment, cancellationToken);
        }

        private async Task<TodayPracticePlanDto> BuildPlanAfterDistancedJournalYesterdayAsync(
            List<ExerciseRecord> yesterdayExerciseRecords,
            bool hasDistancedJournalReflectionToday,
            bool hasPerspectiveScenarioToday,
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            DistancedJournalReflectionPromptDto? reflectionPrompt = null;

            if (!hasDistancedJournalReflectionToday)
            {
                reflectionPrompt = await BuildReflectionPromptFromYesterdayAsync(
                    yesterdayExerciseRecords,
                    cancellationToken);
            }

            var perspectiveScenarioChoices = !hasPerspectiveScenarioToday
                ? await BuildPerspectiveScenarioChoicesForTodayAsync(assignment, cancellationToken)
                : Array.Empty<PerspectiveScenarioPromptDto>();

            var shouldShowPerspectiveScenario = !hasPerspectiveScenarioToday && perspectiveScenarioChoices.Count > 0;

            return BuildPlan(
                reflectionPrompt,
                Array.Empty<DistancedJournalChallengeDto>(),
                perspectiveScenarioChoices,
                shouldShowPerspectiveScenario,
                false,
                hasDistancedJournalReflectionToday,
                hasPerspectiveScenarioToday);
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
