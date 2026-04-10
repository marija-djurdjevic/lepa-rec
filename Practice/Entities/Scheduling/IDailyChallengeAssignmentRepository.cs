using System;
using System.Collections.Generic;

namespace AngularNetBase.Practice.Entities.Scheduling
{
    public interface IDailyChallengeAssignmentRepository
    {
        Task<DailyChallengeAssignment?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Guid>> GetAssignedDistancedJournalChallengeIdsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Guid>> GetAssignedPerspectiveScenarioChallengeIdsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(DailyChallengeAssignment entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
