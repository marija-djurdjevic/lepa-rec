using AngularNetBase.Practice.Entities.DistancedJournals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class DistancedJournalChallengeRepository : IDistancedJournalChallengeRepository
    {
        private readonly PracticeContext _context;

        public DistancedJournalChallengeRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<DistancedJournalChallenge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalChallenges
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<DistancedJournalChallenge>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalChallenges
                .Include(x => x.Questions)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DistancedJournalChallenge>> GetByChallengeLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalChallenges
                .Include(x => x.Questions)
                .Where(x => x.ChallengeLevel == challengeLevel)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(DistancedJournalChallenge entity, CancellationToken cancellationToken = default)
        {
            await _context.DistancedJournalChallenges.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(DistancedJournalChallenge entity, CancellationToken cancellationToken = default)
        {
            _context.DistancedJournalChallenges.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(DistancedJournalChallenge entity, CancellationToken cancellationToken = default)
        {
            _context.DistancedJournalChallenges.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<DistancedJournalChallenge?> GetRandomByLevelAsync(ChallengeLevel challengeLevel, CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalChallenges
                .Include(x => x.Questions)
                .Where(x => x.ChallengeLevel == challengeLevel && !x.IsOnboardingHook)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<DistancedJournalChallenge?> GetOnboardingHookByKeyAsync(string hookKey, CancellationToken cancellationToken = default)
        {
            return await _context.DistancedJournalChallenges
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(
                    x => x.IsOnboardingHook && x.OnboardingHookKey == hookKey,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}
