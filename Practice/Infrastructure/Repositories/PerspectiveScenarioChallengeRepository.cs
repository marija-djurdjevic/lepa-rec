using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class PerspectiveScenarioChallengeRepository : IPerspectiveScenarioChallengeRepository
    {
        private readonly PracticeContext _context;

        public PerspectiveScenarioChallengeRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<PerspectiveScenarioChallenge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioChallenges
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<PerspectiveScenarioChallenge>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioChallenges
                .Include(x => x.Questions)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PerspectiveScenarioChallenge>> GetByChallengeLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioChallenges
                .Include(x => x.Questions)
                .Where(x => x.ChallengeLevel == challengeLevel)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(PerspectiveScenarioChallenge entity, CancellationToken cancellationToken = default)
        {
            await _context.PerspectiveScenarioChallenges.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(PerspectiveScenarioChallenge entity, CancellationToken cancellationToken = default)
        {
            _context.PerspectiveScenarioChallenges.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(PerspectiveScenarioChallenge entity, CancellationToken cancellationToken = default)
        {
            _context.PerspectiveScenarioChallenges.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<PerspectiveScenarioChallenge?> GetRandomByLevelAsync(ChallengeLevel challengeLevel, CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioChallenges
                .Include(x => x.Questions)
                .Where(x => x.ChallengeLevel == challengeLevel)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
