using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IDistancedJournalService
    {
        Task<DistancedJournalChallengeDto> CreateChallengeAsync(
        CreateDistancedJournalChallengeDto dto,
        CancellationToken cancellationToken = default);

        Task<IEnumerable<DistancedJournalChallengeDto>> GetAllChallengesAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DistancedJournalChallengeDto>> GetChallengesByLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalExerciseDto> StartExerciseAsync(
            StartDistancedJournalExerciseDto dto,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalExerciseDto?> GetExerciseByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DistancedJournalExerciseDto>> GetExercisesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<SubmitDistancedJournalResultDto> SubmitAnswerAsync(
        SubmitDistancedJournalAnswerDto dto,
        CancellationToken cancellationToken = default);

        Task<DistancedJournalExerciseDto> AddReflectionAsync(
            AddDistancedJournalReflectionDto dto,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalChallengeDto> GetRandomChallengeAsync(
            ChallengeLevel level,
            CancellationToken cancellationToken = default);
    }
}
