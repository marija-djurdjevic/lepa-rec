using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public interface IDistancedJournalChallengeRepository : IRepository<DistancedJournalChallenge, Guid>
    {
        Task<IEnumerable<DistancedJournalChallenge>> GetByChallengeLevelAsync(
        ChallengeLevel challengeLevel,
        CancellationToken cancellationToken = default);

        Task<DistancedJournalChallenge?> GetRandomByLevelAsync(
        ChallengeLevel challengeLevel,
        CancellationToken cancellationToken = default);


    }

}
