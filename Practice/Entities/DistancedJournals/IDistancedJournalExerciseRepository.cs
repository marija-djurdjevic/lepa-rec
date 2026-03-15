using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public interface IDistancedJournalExerciseRepository : IRepository<DistancedJournalExercise, Guid>
    {
       Task<IEnumerable<DistancedJournalExercise>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

       Task<DistancedJournalExercise?> GetByUserIdAndChallengeIdAsync(Guid userId, Guid challengeId, CancellationToken cancellationToken = default);
    }
}
