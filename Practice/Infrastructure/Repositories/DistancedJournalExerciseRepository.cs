using AngularNetBase.Practice.Entities.DistancedJournals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class DistancedJournalExerciseRepository : IDistancedJournalExerciseRepository
    {
        private readonly PracticeContext _context;

        public DistancedJournalExerciseRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<DistancedJournalExercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalExercises
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<DistancedJournalExercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalExercises
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DistancedJournalExercise>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalExercises
                .Include(x => x.Photos)
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DistancedJournalExercise?> GetByUserIdAndChallengeIdAsync(
            Guid userId,
            Guid challengeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalExercises
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(
                    x => x.UserId == userId && x.ChallengeId == challengeId,
                    cancellationToken);
        }

        public async Task<IReadOnlyCollection<Guid>> GetUsedChallengeIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalExercises
                .Select(x => x.ChallengeId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(DistancedJournalExercise entity, CancellationToken cancellationToken = default)
        {
            await _context.DistancedJournalExercises.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(DistancedJournalExercise entity, CancellationToken cancellationToken = default)
        {
            _context.DistancedJournalExercises.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(DistancedJournalExercise entity, CancellationToken cancellationToken = default)
        {
            _context.DistancedJournalExercises.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
