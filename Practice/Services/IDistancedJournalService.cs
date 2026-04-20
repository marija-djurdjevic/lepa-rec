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
            string? language = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DistancedJournalChallengeDto>> GetChallengesByLevelAsync(
            ChallengeLevel challengeLevel,
            string? language = null,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalExerciseDto> StartExerciseAsync(
            StartDistancedJournalExerciseDto dto,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalExerciseDto?> GetExerciseByIdAsync(
            Guid userId,
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DistancedJournalExerciseDto>> GetExercisesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<SubmitDistancedJournalResultDto> SubmitAnswerAsync(
            Guid userId,
            SubmitDistancedJournalAnswerDto dto,
            CancellationToken cancellationToken = default);

        Task<SubmitDistancedJournalResultDto> SubmitAnswerWithPhotosAsync(
            Guid userId,
            SubmitDistancedJournalAnswerDto dto,
            IReadOnlyCollection<PhotoUpload> photos,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalExerciseDto> AddReflectionAsync(
            Guid userId,
            AddDistancedJournalReflectionDto dto,
            CancellationToken cancellationToken = default);

        Task<(Stream Stream, string ContentType, string FileName)> GetPhotoAsync(
            Guid userId,
            Guid exerciseId,
            Guid photoId,
            CancellationToken cancellationToken = default);

        Task<DistancedJournalChallengeDto> GetRandomChallengeAsync(
            ChallengeLevel level,
            string? language = null,
            CancellationToken cancellationToken = default);
    }
}
