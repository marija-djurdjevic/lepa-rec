using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public interface IPerspectiveScenarioChallengeRepository : IRepository<PerspectiveScenarioChallenge, Guid>
    {
        Task<IEnumerable<PerspectiveScenarioChallenge>> GetByChallengeLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioChallenge?> GetRandomByLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
