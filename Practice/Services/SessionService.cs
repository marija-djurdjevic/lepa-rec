using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Dtos.Sessions;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Entities.Scheduling;
using AngularNetBase.Practice.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
        private readonly PracticeContext _context;

        public SessionService(
            ISessionRepository sessionRepository,
            IDistancedJournalExerciseRepository distancedJournalExerciseRepository,
            IDistancedJournalChallengeRepository distancedJournalChallengeRepository,
            IPerspectiveScenarioChallengeRepository perspectiveScenarioChallengeRepository,
            IPerspectiveScenarioExerciseRepository perspectiveScenarioExerciseRepository,
            IDailyChallengeAssignmentService dailyChallengeAssignmentService,
            IDateTimeProvider dateTimeProvider,
            PracticeContext context)
        {
            _sessionRepository = sessionRepository;
            _distancedJournalExerciseRepository = distancedJournalExerciseRepository;
            _distancedJournalChallengeRepository = distancedJournalChallengeRepository;
            _perspectiveScenarioChallengeRepository = perspectiveScenarioChallengeRepository;
            _perspectiveScenarioExerciseRepository = perspectiveScenarioExerciseRepository;
            _dailyChallengeAssignmentService = dailyChallengeAssignmentService;
            _dateTimeProvider = dateTimeProvider;
            _context = context;
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
            string? lang = null,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId must be provided.");

            var isEnglish = IsEnglishLanguage(lang);
            var today = _dateTimeProvider.BusinessDate;
            var assignment = await GetOrCreateUserDailyAssignmentAsync(userId, today, cancellationToken);

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

            var reflectionPrompt = !hasDistancedJournalReflectionToday && assignment.ReflectionExerciseId.HasValue
                ? await BuildReflectionPromptAsync(assignment.ReflectionExerciseId.Value, isEnglish, cancellationToken)
                : null;

            var distancedJournalChoices = assignment.MainExerciseType == ExerciseType.DistancedJournal && !hasDistancedJournalToday
                ? await BuildDistancedJournalChoicesForUserAssignmentAsync(assignment, isEnglish, cancellationToken)
                : Array.Empty<DistancedJournalChallengeDto>();

            var perspectiveScenarioChoices = assignment.MainExerciseType == ExerciseType.PerspectiveScenario && !hasPerspectiveScenarioToday
                ? await BuildPerspectiveScenarioChoicesForUserAssignmentAsync(assignment, isEnglish, cancellationToken)
                : Array.Empty<PerspectiveScenarioPromptDto>();

            return BuildPlan(
                reflectionPrompt,
                distancedJournalChoices,
                perspectiveScenarioChoices,
                assignment.MainExerciseType == ExerciseType.PerspectiveScenario && perspectiveScenarioChoices.Count > 0,
                hasDistancedJournalToday,
                hasDistancedJournalReflectionToday,
                hasPerspectiveScenarioToday);
        }

        private async Task<DailySession> GetOrCreateTodaySessionEntityAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var today = _dateTimeProvider.BusinessDate;

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

        private async Task<UserDailyPracticeAssignment> GetOrCreateUserDailyAssignmentAsync(
            Guid userId,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var existing = await _context.UserDailyPracticeAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date.Date, cancellationToken);

            if (existing is not null)
                return existing;

            var reflectionExercise = await FindPendingReflectionExerciseAsync(userId, date, cancellationToken);
            var mainExerciseType = await DetermineNextMainExerciseTypeAsync(userId, cancellationToken);

            var distancedJournalIds = mainExerciseType == ExerciseType.DistancedJournal
                ? await SelectDistancedJournalOptionsForUserAsync(userId, date, cancellationToken)
                : Array.Empty<Guid>();

            var perspectiveScenarioIds = mainExerciseType == ExerciseType.PerspectiveScenario
                ? await SelectPerspectiveScenarioOptionsForUserAsync(userId, date, cancellationToken)
                : Array.Empty<Guid>();

            var assignment = new UserDailyPracticeAssignment(
                Guid.NewGuid(),
                userId,
                date,
                mainExerciseType,
                distancedJournalIds.ElementAtOrDefault(0) == Guid.Empty ? null : distancedJournalIds.ElementAtOrDefault(0),
                distancedJournalIds.ElementAtOrDefault(1) == Guid.Empty ? null : distancedJournalIds.ElementAtOrDefault(1),
                perspectiveScenarioIds.ElementAtOrDefault(0) == Guid.Empty ? null : perspectiveScenarioIds.ElementAtOrDefault(0),
                perspectiveScenarioIds.ElementAtOrDefault(1) == Guid.Empty ? null : perspectiveScenarioIds.ElementAtOrDefault(1),
                reflectionExercise?.Id,
                _dateTimeProvider.UtcNow);

            _context.UserDailyPracticeAssignments.Add(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return assignment;
        }

        private async Task<ExerciseType> DetermineNextMainExerciseTypeAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var sessions = await _context.DailySessions
                .Include(x => x.Events)
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            var lastMainExercise = sessions
                .SelectMany(session => session.Events
                    .OfType<ExerciseRecord>()
                    .Where(record => record.Type is ExerciseType.DistancedJournal or ExerciseType.PerspectiveScenario)
                    .Select(record => new { session.Date, Record = record }))
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Record.Timestamp)
                .FirstOrDefault();

            return lastMainExercise?.Record.Type == ExerciseType.DistancedJournal
                ? ExerciseType.PerspectiveScenario
                : ExerciseType.DistancedJournal;
        }

        private async Task<DistancedJournalExercise?> FindPendingReflectionExerciseAsync(
            Guid userId,
            DateTime today,
            CancellationToken cancellationToken)
        {
            // Npgsql rejects DateTimeKind.Unspecified for timestamptz parameters.
            // This cutoff is compared against SubmittedAt (timestamptz), so force UTC kind.
            var cutoff = DateTime.SpecifyKind(today.Date.AddDays(-6), DateTimeKind.Utc);

            return await _context.DistancedJournalExercises
                .Include(x => x.Photos)
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsOnboardingHookRun &&
                    x.Answer != null &&
                    x.Answer.Reflection == null &&
                    x.Answer.SubmittedAt < cutoff)
                .OrderBy(x => x.Answer!.SubmittedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task<IReadOnlyCollection<Guid>> SelectDistancedJournalOptionsForUserAsync(
            Guid userId,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var challenges = await _context.DistancedJournalChallenges
                .Include(x => x.Questions)
                .Where(x => !x.IsOnboardingHook)
                .ToListAsync(cancellationToken);

            var seenIds = await _context.UserChallengeExposures
                .Where(x => x.UserId == userId && x.Type == ChallengeExposureType.DistancedJournal)
                .Select(x => x.ChallengeId)
                .ToListAsync(cancellationToken);

            var solvedIds = await _context.DistancedJournalExercises
                .Where(x => x.UserId == userId && x.Answer != null)
                .Select(x => x.ChallengeId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var selected = SelectJournalPair(challenges, seenIds.ToHashSet(), solvedIds.ToHashSet());
            AddMissingExposures(userId, ChallengeExposureType.DistancedJournal, selected, date);
            return selected;
        }

        private async Task<IReadOnlyCollection<Guid>> SelectPerspectiveScenarioOptionsForUserAsync(
            Guid userId,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var challenges = await _context.PerspectiveScenarioChallenges
                .Include(x => x.Questions)
                .Where(x => !x.IsOnboardingHook)
                .ToListAsync(cancellationToken);

            var seenIds = await _context.UserChallengeExposures
                .Where(x => x.UserId == userId && x.Type == ChallengeExposureType.PerspectiveScenario)
                .Select(x => x.ChallengeId)
                .ToListAsync(cancellationToken);

            var solvedIds = await _context.PerspectiveScenarioExercises
                .Where(x => x.UserId == userId && x.SubmittedAt != null)
                .Select(x => x.ChallengeId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var selected = SelectPairWithEasy(
                challenges
                    .Where(x => !solvedIds.Contains(x.Id))
                    .Select(x => new OptionCandidate(x.Id, x.ChallengeLevel, seenIds.Contains(x.Id) ? 1 : 0))
                    .ToList());

            if (selected.Count < 2)
            {
                selected = SelectPairWithEasy(
                    challenges
                        .Select(x => new OptionCandidate(x.Id, x.ChallengeLevel, solvedIds.Contains(x.Id) ? 2 : seenIds.Contains(x.Id) ? 1 : 0))
                        .ToList());
            }

            AddMissingExposures(userId, ChallengeExposureType.PerspectiveScenario, selected, date);
            return selected;
        }

        private void AddMissingExposures(
            Guid userId,
            ChallengeExposureType type,
            IReadOnlyCollection<Guid> challengeIds,
            DateTime shownOnDate)
        {
            var existing = _context.UserChallengeExposures
                .Where(x => x.UserId == userId && x.Type == type)
                .Select(x => x.ChallengeId)
                .ToHashSet();

            foreach (var challengeId in challengeIds.Distinct())
            {
                if (challengeId == Guid.Empty || existing.Contains(challengeId))
                    continue;

                _context.UserChallengeExposures.Add(new UserChallengeExposure(
                    Guid.NewGuid(),
                    userId,
                    type,
                    challengeId,
                    shownOnDate,
                    _dateTimeProvider.UtcNow));
            }
        }

        private static IReadOnlyCollection<Guid> SelectJournalPair(
            IReadOnlyCollection<DistancedJournalChallenge> challenges,
            HashSet<Guid> seenIds,
            HashSet<Guid> solvedIds)
        {
            var phases = new[]
            {
                DistancedJournalPhase.A,
                DistancedJournalPhase.Single,
                DistancedJournalPhase.B,
                DistancedJournalPhase.Single
            };

            var candidates = new List<OptionCandidate>();

            foreach (var phase in phases)
            {
                var phaseCandidates = challenges
                    .Where(x => x.Phase == phase && !solvedIds.Contains(x.Id))
                    .Select(x => new OptionCandidate(x.Id, x.ChallengeLevel, seenIds.Contains(x.Id) ? 1 : 0))
                    .OrderBy(x => x.Priority)
                    .ThenBy(_ => Random.Shared.Next())
                    .ToList();

                foreach (var candidate in phaseCandidates)
                {
                    if (candidates.Any(x => x.Id == candidate.Id))
                        continue;

                    candidates.Add(candidate);
                    if (candidates.Count >= 2)
                        return EnsureEasyCandidate(candidates, BuildAllJournalCandidates(challenges, seenIds, solvedIds));
                }
            }

            if (candidates.Count >= 2)
                return EnsureEasyCandidate(candidates, BuildAllJournalCandidates(challenges, seenIds, solvedIds));

            var fallback = challenges
                .Select(x => new OptionCandidate(x.Id, x.ChallengeLevel, solvedIds.Contains(x.Id) ? 2 : seenIds.Contains(x.Id) ? 1 : 0))
                .OrderBy(x => x.Priority)
                .ThenBy(_ => Random.Shared.Next())
                .ToList();

            return SelectPairWithEasy(fallback);
        }

        private static List<OptionCandidate> BuildAllJournalCandidates(
            IReadOnlyCollection<DistancedJournalChallenge> challenges,
            HashSet<Guid> seenIds,
            HashSet<Guid> solvedIds)
        {
            return challenges
                .Where(x => !solvedIds.Contains(x.Id))
                .Select(x => new OptionCandidate(x.Id, x.ChallengeLevel, seenIds.Contains(x.Id) ? 1 : 0))
                .OrderBy(x => x.Priority)
                .ThenBy(_ => Random.Shared.Next())
                .ToList();
        }

        private static IReadOnlyCollection<Guid> SelectPairWithEasy(List<OptionCandidate> candidates)
        {
            var selected = candidates
                .OrderBy(x => x.Priority)
                .ThenBy(_ => Random.Shared.Next())
                .DistinctBy(x => x.Id)
                .Take(2)
                .ToList();

            return EnsureEasyCandidate(selected, candidates);
        }

        private static IReadOnlyCollection<Guid> EnsureEasyCandidate(
            List<OptionCandidate> selected,
            List<OptionCandidate> candidates)
        {
            if (selected.Count == 0)
                return Array.Empty<Guid>();

            if (selected.Any(x => x.Level == ChallengeLevel.Easy))
                return selected.Select(x => x.Id).ToList();

            var easyCandidate = candidates
                .Where(x => x.Level == ChallengeLevel.Easy && selected.All(s => s.Id != x.Id))
                .OrderBy(x => x.Priority)
                .ThenBy(_ => Random.Shared.Next())
                .FirstOrDefault();

            if (easyCandidate is not null)
            {
                if (selected.Count == 1)
                    selected.Add(easyCandidate);
                else
                    selected[^1] = easyCandidate;
            }

            return selected
                .DistinctBy(x => x.Id)
                .Take(2)
                .Select(x => x.Id)
                .ToList();
        }

        private async Task<IReadOnlyCollection<DistancedJournalChallengeDto>> BuildDistancedJournalChoicesForUserAssignmentAsync(
            UserDailyPracticeAssignment assignment,
            bool isEnglish,
            CancellationToken cancellationToken)
        {
            var ids = new[] { assignment.DistancedJournalChallengeId, assignment.DistancedJournalChallengeId2 }
                .Where(id => id.HasValue && id.Value != Guid.Empty)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var choices = new List<DistancedJournalChallengeDto>();
            foreach (var id in ids)
            {
                var challenge = await _distancedJournalChallengeRepository.GetByIdAsync(id, cancellationToken);
                if (challenge is not null)
                    choices.Add(MapDistancedJournalChallenge(challenge, isEnglish));
            }

            return choices;
        }

        private async Task<IReadOnlyCollection<PerspectiveScenarioPromptDto>> BuildPerspectiveScenarioChoicesForUserAssignmentAsync(
            UserDailyPracticeAssignment assignment,
            bool isEnglish,
            CancellationToken cancellationToken)
        {
            var ids = new[] { assignment.PerspectiveScenarioChallengeId, assignment.PerspectiveScenarioChallengeId2 }
                .Where(id => id.HasValue && id.Value != Guid.Empty)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var choices = new List<PerspectiveScenarioPromptDto>();
            foreach (var id in ids)
            {
                var challenge = await _perspectiveScenarioChallengeRepository.GetByIdAsync(id, cancellationToken);
                if (challenge is not null)
                    choices.Add(MapPerspectiveScenarioPrompt(challenge, isEnglish));
            }

            return choices;
        }

        private async Task<DistancedJournalReflectionPromptDto?> BuildReflectionPromptAsync(
            Guid exerciseId,
            bool isEnglish,
            CancellationToken cancellationToken)
        {
            var exercise = await _distancedJournalExerciseRepository.GetByIdAsync(exerciseId, cancellationToken);
            if (exercise is null || exercise.Answer is null)
                return null;

            var challenge = await _distancedJournalChallengeRepository.GetByIdAsync(exercise.ChallengeId, cancellationToken);
            if (challenge is null)
                return null;

            var reflectionQuestion = challenge.Questions
                .FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.Reflection);
            var openingQuestion = challenge.Questions
                .FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.Opening);
            var followUpQuestion = challenge.Questions
                .FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.FollowUp);

            return new DistancedJournalReflectionPromptDto(
                exercise.Id,
                Localize(challenge.Content, challenge.ContentEn, isEnglish),
                openingQuestion is null
                    ? Localize(challenge.Content, challenge.ContentEn, isEnglish)
                    : Localize(openingQuestion.Text, openingQuestion.TextEn, isEnglish),
                followUpQuestion is null
                    ? Localize(challenge.FollowUpQuestion, challenge.FollowUpQuestionEn, isEnglish)
                    : Localize(followUpQuestion.Text, followUpQuestion.TextEn, isEnglish),
                Localize(challenge.FollowUpQuestion, challenge.FollowUpQuestionEn, isEnglish),
                exercise.Answer.MainAnswer,
                exercise.Answer.FollowUpAnswer,
                exercise.Photos
                    .Select(p => $"/api/DistancedJournals/{exercise.Id}/photos/{p.Id}")
                    .ToList(),
                reflectionQuestion is null
                    ? null
                    : Localize(reflectionQuestion.Text, reflectionQuestion.TextEn, isEnglish));
        }

        private sealed record OptionCandidate(Guid Id, ChallengeLevel Level, int Priority);

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
            bool isEnglish,
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
                Localize(challenge.Content, challenge.ContentEn, isEnglish),
                challenge.Questions.FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.Opening) is DistancedJournalQuestion openingQuestion
                    ? Localize(openingQuestion.Text, openingQuestion.TextEn, isEnglish)
                    : Localize(challenge.Content, challenge.ContentEn, isEnglish),
                challenge.Questions.FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.FollowUp) is DistancedJournalQuestion followUpQuestion
                    ? Localize(followUpQuestion.Text, followUpQuestion.TextEn, isEnglish)
                    : Localize(challenge.FollowUpQuestion, challenge.FollowUpQuestionEn, isEnglish),
                Localize(challenge.FollowUpQuestion, challenge.FollowUpQuestionEn, isEnglish),
                exercise.Answer.MainAnswer,
                exercise.Answer.FollowUpAnswer,
                exercise.Photos
                    .Select(p => $"/api/DistancedJournals/{exercise.Id}/photos/{p.Id}")
                    .ToList());
        }

        private async Task<IReadOnlyCollection<DistancedJournalChallengeDto>> BuildDistancedJournalChoicesForTodayAsync(
            DailyChallengeAssignment assignment,
            bool isEnglish,
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

                choices.Add(MapDistancedJournalChallenge(challenge, isEnglish));
            }

            if (choices.Count < 2)
            {
                var fallback = await BuildFallbackDistancedJournalChoicesAsync(
                    choices.Select(x => x.Id).ToHashSet(),
                    isEnglish,
                    cancellationToken);

                choices.AddRange(fallback);
            }

            return choices.Take(2).ToList();
        }

        private async Task<IReadOnlyCollection<DistancedJournalChallengeDto>> BuildFallbackDistancedJournalChoicesAsync(
            HashSet<Guid> excludeIds,
            bool isEnglish,
            CancellationToken cancellationToken)
        {
            var challenges = await _distancedJournalChallengeRepository.GetAllAsync(cancellationToken);
            var available = challenges
                .Where(x => !excludeIds.Contains(x.Id) && !x.IsOnboardingHook)
                .ToList();

            if (available.Count == 0)
                return Array.Empty<DistancedJournalChallengeDto>();

            var first = available[Random.Shared.Next(available.Count)];
            var results = new List<DistancedJournalChallengeDto>
            {
                MapDistancedJournalChallenge(first, isEnglish)
            };

            var remaining = available.Where(x => x.Id != first.Id).ToList();
            if (remaining.Count > 0)
            {
                var second = remaining[Random.Shared.Next(remaining.Count)];
                results.Add(MapDistancedJournalChallenge(second, isEnglish));
            }

            return results;
        }

        private async Task<IReadOnlyCollection<PerspectiveScenarioPromptDto>> BuildPerspectiveScenarioChoicesForTodayAsync(
            DailyChallengeAssignment assignment,
            bool isEnglish,
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

                choices.Add(MapPerspectiveScenarioPrompt(challenge, isEnglish));
            }

            if (choices.Count < 2)
            {
                var fallback = await BuildPerspectiveScenarioChoicesFallbackAsync(
                    choices.Select(x => x.Id).ToHashSet(),
                    isEnglish,
                    cancellationToken);

                choices.AddRange(fallback);
            }

            return choices.Take(2).ToList();
        }

        private async Task<IReadOnlyCollection<PerspectiveScenarioPromptDto>> BuildPerspectiveScenarioChoicesFallbackAsync(
            HashSet<Guid> excludeIds,
            bool isEnglish,
            CancellationToken cancellationToken)
        {
            var challenges = await _perspectiveScenarioChallengeRepository.GetAllAsync(cancellationToken);
            var available = challenges
                .Where(x => !excludeIds.Contains(x.Id) && !x.IsOnboardingHook)
                .ToList();

            if (available.Count == 0)
                return Array.Empty<PerspectiveScenarioPromptDto>();

            var first = available[Random.Shared.Next(available.Count)];
            var results = new List<PerspectiveScenarioPromptDto>
            {
                MapPerspectiveScenarioPrompt(first, isEnglish)
            };

            var remaining = available.Where(x => x.Id != first.Id).ToList();
            if (remaining.Count > 0)
            {
                var second = remaining[Random.Shared.Next(remaining.Count)];
                results.Add(MapPerspectiveScenarioPrompt(second, isEnglish));
            }

            return results;
        }

        private static PerspectiveScenarioPromptDto MapPerspectiveScenarioPrompt(PerspectiveScenarioChallenge challenge, bool isEnglish)
        {
            return new PerspectiveScenarioPromptDto(
                challenge.Id,
                challenge.ActorCount,
                challenge.Context,
                Localize(challenge.ScenarioText, challenge.ScenarioTextEn, isEnglish),
                challenge.ChallengeLevel,
                challenge.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => new PerspectiveScenarioPromptQuestionDto(
                        q.Id,
                        q.SkillId,
                        q.Order,
                        Localize(q.QuestionText, q.QuestionTextEn, isEnglish)))
                    .ToList());
        }

        private static DistancedJournalChallengeDto MapDistancedJournalChallenge(DistancedJournalChallenge challenge, bool isEnglish)
        {
            var openingQuestion = challenge.Questions.FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.Opening);
            var followUpQuestion = challenge.Questions.FirstOrDefault(x => x.Kind == DistancedJournalQuestionKind.FollowUp);

            return new DistancedJournalChallengeDto(
                challenge.Id,
                challenge.Theme,
                challenge.Variant,
                challenge.Phase,
                Localize(challenge.Content, challenge.ContentEn, isEnglish),
                openingQuestion is null
                    ? Localize(challenge.Content, challenge.ContentEn, isEnglish)
                    : Localize(openingQuestion.Text, openingQuestion.TextEn, isEnglish),
                followUpQuestion is null
                    ? Localize(challenge.FollowUpQuestion, challenge.FollowUpQuestionEn, isEnglish)
                    : Localize(followUpQuestion.Text, followUpQuestion.TextEn, isEnglish),
                challenge.ChallengeLevel,
                challenge.SkillId,
                challenge.Questions
                    .OrderBy(x => x.Order)
                    .Select(question => new DistancedJournalQuestionDto(
                        question.Id,
                        question.Kind,
                        question.Order,
                        Localize(question.Text, question.TextEn, isEnglish),
                        question.SkillId))
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
            bool isEnglish,
            CancellationToken cancellationToken)
        {
            return BuildPlan(
                null,
                await BuildDistancedJournalChoicesForTodayAsync(assignment, isEnglish, cancellationToken),
                Array.Empty<PerspectiveScenarioPromptDto>(),
                false,
                false,
                false,
                false);
        }

        private async Task<TodayPracticePlanDto> BuildPlanBasedOnDistancedJournalTodayAsync(
            bool hasDistancedJournalToday,
            bool isEnglish,
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            if (hasDistancedJournalToday)
                return BuildPlanWithDistancedJournalCompletedOnly();

            return await BuildPlanWithDistancedJournalChoicesAsync(assignment, isEnglish, cancellationToken);
        }

        private async Task<TodayPracticePlanDto> BuildPlanAfterDistancedJournalYesterdayAsync(
            List<ExerciseRecord> yesterdayExerciseRecords,
            bool hasDistancedJournalReflectionToday,
            bool hasPerspectiveScenarioToday,
            bool isEnglish,
            DailyChallengeAssignment assignment,
            CancellationToken cancellationToken)
        {
            DistancedJournalReflectionPromptDto? reflectionPrompt = null;

            if (!hasDistancedJournalReflectionToday)
            {
                reflectionPrompt = await BuildReflectionPromptFromYesterdayAsync(
                    yesterdayExerciseRecords,
                    isEnglish,
                    cancellationToken);
            }

            var perspectiveScenarioChoices = !hasPerspectiveScenarioToday
                ? await BuildPerspectiveScenarioChoicesForTodayAsync(assignment, isEnglish, cancellationToken)
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

        private static string Localize(string sourceText, string? englishText, bool isEnglish)
            => isEnglish && !string.IsNullOrWhiteSpace(englishText) ? englishText : sourceText;

        private static bool IsEnglishLanguage(string? lang)
            => string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase);
    }
}
