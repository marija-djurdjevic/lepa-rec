using AngularNetBase.Practice.Entities.Scheduling;
using System;

namespace AngularNetBase.Practice.Services
{
    public interface IDailyChallengeAssignmentService
    {
        Task<DailyChallengeAssignment> GetOrCreateTodayAssignmentAsync(CancellationToken cancellationToken = default);
        Task EnsureAssignmentForDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
