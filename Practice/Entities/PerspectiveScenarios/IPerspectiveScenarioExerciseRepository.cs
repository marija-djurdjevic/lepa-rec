using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public interface IPerspectiveScenarioExerciseRepository : IRepository<PerspectiveScenarioExercise, Guid>
    {
        Task<IEnumerable<PerspectiveScenarioExercise>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioExercise?> GetByUserIdAndChallengeIdAsync(
            Guid userId,
            Guid challengeId,
            CancellationToken cancellationToken = default);
    }
}
