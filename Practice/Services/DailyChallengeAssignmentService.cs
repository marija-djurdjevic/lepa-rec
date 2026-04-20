using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AngularNetBase.Practice.Services
{
    public class DailyChallengeAssignmentService : IDailyChallengeAssignmentService
    {
        private readonly IDailyChallengeAssignmentRepository _assignmentRepository;
        private readonly IDistancedJournalChallengeRepository _distancedJournalChallengeRepository;
        private readonly IPerspectiveScenarioChallengeRepository _perspectiveScenarioChallengeRepository;
        private readonly IDistancedJournalExerciseRepository _distancedJournalExerciseRepository;
        private readonly IPerspectiveScenarioExerciseRepository _perspectiveScenarioExerciseRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DailyChallengeAssignmentService(
            IDailyChallengeAssignmentRepository assignmentRepository,
            IDistancedJournalChallengeRepository distancedJournalChallengeRepository,
            IPerspectiveScenarioChallengeRepository perspectiveScenarioChallengeRepository,
            IDistancedJournalExerciseRepository distancedJournalExerciseRepository,
            IPerspectiveScenarioExerciseRepository perspectiveScenarioExerciseRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _assignmentRepository = assignmentRepository;
            _distancedJournalChallengeRepository = distancedJournalChallengeRepository;
            _perspectiveScenarioChallengeRepository = perspectiveScenarioChallengeRepository;
            _distancedJournalExerciseRepository = distancedJournalExerciseRepository;
            _perspectiveScenarioExerciseRepository = perspectiveScenarioExerciseRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<DailyChallengeAssignment> GetOrCreateTodayAssignmentAsync(CancellationToken cancellationToken = default)
        {
            var today = _dateTimeProvider.BusinessDate;
            return await GetOrCreateAssignmentForDateAsync(today, cancellationToken);
        }

        public async Task EnsureAssignmentForDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            await GetOrCreateAssignmentForDateAsync(date.Date, cancellationToken);
        }

        private async Task<DailyChallengeAssignment> GetOrCreateAssignmentForDateAsync(DateTime date, CancellationToken cancellationToken)
        {
            var existing = await _assignmentRepository.GetByDateAsync(date, cancellationToken);
            if (existing is not null)
            {
                if (existing.DistancedJournalChallengeId2 == Guid.Empty || existing.PerspectiveScenarioChallengeId2 == Guid.Empty)
                {
                    var distancedJournalId2 = existing.DistancedJournalChallengeId2 == Guid.Empty
                        ? await SelectSecondDistancedJournalChallengeIdAsync(existing.DistancedJournalChallengeId, cancellationToken)
                        : existing.DistancedJournalChallengeId2;

                    var perspectiveScenarioId2 = existing.PerspectiveScenarioChallengeId2 == Guid.Empty
                        ? await SelectSecondPerspectiveScenarioChallengeIdAsync(existing.PerspectiveScenarioChallengeId, cancellationToken)
                        : existing.PerspectiveScenarioChallengeId2;

                    existing.EnsureSecondOptions(distancedJournalId2, perspectiveScenarioId2);
                    await _assignmentRepository.SaveChangesAsync(cancellationToken);
                }

                return existing;
            }

            var distancedJournalIds = await SelectDistancedJournalChallengeIdsAsync(cancellationToken);
            var perspectiveScenarioIds = await SelectPerspectiveScenarioChallengeIdsAsync(cancellationToken);

            var assignment = new DailyChallengeAssignment(
                Guid.NewGuid(),
                date,
                distancedJournalIds.First,
                distancedJournalIds.Second,
                perspectiveScenarioIds.First,
                perspectiveScenarioIds.Second,
                _dateTimeProvider.UtcNow);

            await _assignmentRepository.AddAsync(assignment, cancellationToken);

            try
            {
                await _assignmentRepository.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                var stored = await _assignmentRepository.GetByDateAsync(date, cancellationToken);
                if (stored is not null)
                    return stored;

                throw;
            }

            return assignment;
        }

        private async Task<(Guid First, Guid Second)> SelectDistancedJournalChallengeIdsAsync(CancellationToken cancellationToken)
        {
            var challenges = (await _distancedJournalChallengeRepository.GetAllAsync(cancellationToken)).ToList();
            if (challenges.Count == 0)
                throw new InvalidOperationException("No distanced journal challenges found.");

            var used = await _distancedJournalExerciseRepository.GetUsedChallengeIdsAsync(cancellationToken);
            var available = challenges.Select(x => x.Id).Where(id => !used.Contains(id)).ToList();
            if (available.Count == 0)
                available = challenges.Select(x => x.Id).ToList();

            return PickTwoDistinct(available);
        }

        private async Task<(Guid First, Guid Second)> SelectPerspectiveScenarioChallengeIdsAsync(CancellationToken cancellationToken)
        {
            var challenges = (await _perspectiveScenarioChallengeRepository.GetAllAsync(cancellationToken)).ToList();
            if (challenges.Count == 0)
                throw new InvalidOperationException("No perspective scenario challenges found.");

            var used = await _perspectiveScenarioExerciseRepository.GetUsedChallengeIdsAsync(cancellationToken);
            var available = challenges.Select(x => x.Id).Where(id => !used.Contains(id)).ToList();
            if (available.Count == 0)
                available = challenges.Select(x => x.Id).ToList();

            return PickTwoDistinct(available);
        }

        private static (Guid First, Guid Second) PickTwoDistinct(List<Guid> available)
        {
            if (available.Count == 1)
            {
                var only = available[0];
                return (only, only);
            }

            var firstIndex = Random.Shared.Next(available.Count);
            var first = available[firstIndex];

            Guid second;
            do
            {
                second = available[Random.Shared.Next(available.Count)];
            } while (second == first);

            return (first, second);
        }

        private async Task<Guid> SelectSecondDistancedJournalChallengeIdAsync(Guid firstId, CancellationToken cancellationToken)
        {
            var challenges = (await _distancedJournalChallengeRepository.GetAllAsync(cancellationToken)).ToList();
            if (challenges.Count == 0)
                throw new InvalidOperationException("No distanced journal challenges found.");

            var used = await _distancedJournalExerciseRepository.GetUsedChallengeIdsAsync(cancellationToken);
            var candidates = challenges.Select(x => x.Id).Where(id => id != firstId && !used.Contains(id)).ToList();
            if (candidates.Count == 0)
                candidates = challenges.Select(x => x.Id).Where(id => id != firstId).ToList();
            if (candidates.Count == 0)
                return firstId;

            return candidates[Random.Shared.Next(candidates.Count)];
        }

        private async Task<Guid> SelectSecondPerspectiveScenarioChallengeIdAsync(Guid firstId, CancellationToken cancellationToken)
        {
            var challenges = (await _perspectiveScenarioChallengeRepository.GetAllAsync(cancellationToken)).ToList();
            if (challenges.Count == 0)
                throw new InvalidOperationException("No perspective scenario challenges found.");

            var used = await _perspectiveScenarioExerciseRepository.GetUsedChallengeIdsAsync(cancellationToken);
            var candidates = challenges.Select(x => x.Id).Where(id => id != firstId && !used.Contains(id)).ToList();
            if (candidates.Count == 0)
                candidates = challenges.Select(x => x.Id).Where(id => id != firstId).ToList();
            if (candidates.Count == 0)
                return firstId;

            return candidates[Random.Shared.Next(candidates.Count)];
        }
    }
}
