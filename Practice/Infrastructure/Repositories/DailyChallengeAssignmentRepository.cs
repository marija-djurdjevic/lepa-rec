using AngularNetBase.Practice.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class DailyChallengeAssignmentRepository : IDailyChallengeAssignmentRepository
    {
        private readonly PracticeContext _context;

        public DailyChallengeAssignmentRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<DailyChallengeAssignment?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var targetDate = date.Date;
            return await _context.DailyChallengeAssignments
                .FirstOrDefaultAsync(x => x.Date == targetDate, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Guid>> GetAssignedDistancedJournalChallengeIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DailyChallengeAssignments
                .SelectMany(x => new[] { x.DistancedJournalChallengeId, x.DistancedJournalChallengeId2 })
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Guid>> GetAssignedPerspectiveScenarioChallengeIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DailyChallengeAssignments
                .SelectMany(x => new[] { x.PerspectiveScenarioChallengeId, x.PerspectiveScenarioChallengeId2 })
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(DailyChallengeAssignment entity, CancellationToken cancellationToken = default)
        {
            await _context.DailyChallengeAssignments.AddAsync(entity, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
