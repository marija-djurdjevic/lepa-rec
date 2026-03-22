using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IPerspectiveScenarioService
    {
        Task<PerspectiveScenarioChallengeDto> CreateChallengeAsync(
            CreatePerspectiveScenarioChallengeDto dto,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<PerspectiveScenarioChallengeDto>> GetAllChallengesAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<PerspectiveScenarioChallengeDto>> GetChallengesByLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioPromptDto> GetRandomChallengeAsync(
            ChallengeLevel level,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioExerciseDto> StartExerciseAsync(
            StartPerspectiveScenarioExerciseDto dto,
            CancellationToken cancellationToken = default);

        Task<PerspectiveScenarioExerciseDto?> GetExerciseByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<PerspectiveScenarioExerciseDto>> GetExercisesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<SubmitPerspectiveScenarioResultDto> SubmitAnswersAsync(
            SubmitPerspectiveScenarioAnswerDto dto,
            CancellationToken cancellationToken = default);
    }
}
