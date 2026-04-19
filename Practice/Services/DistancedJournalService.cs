using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.DistancedJournals.Analysis;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Shared.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class DistancedJournalService : IDistancedJournalService
    {
        private readonly IDistancedJournalChallengeRepository _challengeRepository;
        private readonly IDistancedJournalExerciseRepository _exerciseRepository;
        private readonly ISessionRepository _dailySessionRepository;
        private readonly IThirdPersonAnalyzer _thirdPersonAnalyzer;
        private readonly IUserProfileReader _userProfileReader;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJournalPhotoStorage _photoStorage;
        private const int MaxPhotosPerAnswer = 3;

        public DistancedJournalService(
            IDistancedJournalChallengeRepository challengeRepository,
            IDistancedJournalExerciseRepository exerciseRepository,
            ISessionRepository sessionRepository,
            IThirdPersonAnalyzer thirdPersonAnalyzer,
            IUserProfileReader userProfileReader,
            IDateTimeProvider dateTimeProvider,
            IJournalPhotoStorage photoStorage)
        {
            _challengeRepository = challengeRepository;
            _exerciseRepository = exerciseRepository;
            _dailySessionRepository = sessionRepository;
            _thirdPersonAnalyzer = thirdPersonAnalyzer;
            _userProfileReader = userProfileReader;
            _dateTimeProvider = dateTimeProvider;
            _photoStorage = photoStorage;
        }

        public async Task<DistancedJournalChallengeDto> CreateChallengeAsync(
            CreateDistancedJournalChallengeDto dto,
            CancellationToken cancellationToken = default)
        {
            var challenge = new DistancedJournalChallenge(
                Guid.NewGuid(),
                dto.Content,
                dto.FollowUpQuestion,
                dto.ChallengeLevel,
                dto.SkillId);

            await _challengeRepository.AddAsync(challenge, cancellationToken);
            await _challengeRepository.SaveChangesAsync(cancellationToken);

            return MapChallenge(challenge);
        }

        public async Task<IEnumerable<DistancedJournalChallengeDto>> GetAllChallengesAsync(
            CancellationToken cancellationToken = default)
        {
            var challenges = await _challengeRepository.GetAllAsync(cancellationToken);
            return challenges.Select(MapChallenge);
        }

        public async Task<IEnumerable<DistancedJournalChallengeDto>> GetChallengesByLevelAsync(
            ChallengeLevel challengeLevel,
            CancellationToken cancellationToken = default)
        {
            var challenges = await _challengeRepository.GetByChallengeLevelAsync(
                challengeLevel,
                cancellationToken);

            return challenges.Select(MapChallenge);
        }

        public async Task<DistancedJournalExerciseDto> StartExerciseAsync(
            StartDistancedJournalExerciseDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.UserId == Guid.Empty)
                throw new ArgumentException("UserId must be provided.");

            if (dto.ChallengeId == Guid.Empty)
                throw new ArgumentException("ChallengeId must be provided.");

            var challenge = await _challengeRepository.GetByIdAsync(dto.ChallengeId, cancellationToken);
            if (challenge is null)
                throw new InvalidOperationException("Distanced journal challenge was not found.");

            var exercise = new DistancedJournalExercise(
                Guid.NewGuid(),
                dto.UserId,
                dto.ChallengeId);

            await _exerciseRepository.AddAsync(exercise, cancellationToken);
            await _exerciseRepository.SaveChangesAsync(cancellationToken);

            return MapExercise(exercise);
        }

        public async Task<DistancedJournalExerciseDto?> GetExerciseByIdAsync(
            Guid userId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Exercise id must be provided.");

            var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
            if (exercise is null || exercise.UserId != userId)
                return null;

            return exercise is null ? null : MapExercise(exercise);
        }

        public async Task<IEnumerable<DistancedJournalExerciseDto>> GetExercisesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.");

            var exercises = await _exerciseRepository.GetByUserIdAsync(userId, cancellationToken);
            return exercises.Select(MapExercise);
        }

        public async Task<SubmitDistancedJournalResultDto> SubmitAnswerAsync(
            Guid userId,
            SubmitDistancedJournalAnswerDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.");

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Distanced journal exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var dailySession = await GetOrCreateTodaySessionAsync(exercise.UserId, cancellationToken);

            var submittedAt = _dateTimeProvider.UtcNow;

            ValidateTextAnswer(dto.MainAnswer, dto.FollowUpAnswer);

            exercise.SubmitAnswer(
                dto.MainAnswer,
                dto.FollowUpAnswer,
                dto.Reflection,
                submittedAt);

            dailySession.RecordExercise(
                exercise.Id,
                ExerciseType.DistancedJournal,
                submittedAt);

            await SaveSessionWithRetryAsync(
                dailySession,
                exercise.Id,
                ExerciseType.DistancedJournal,
                submittedAt,
                cancellationToken);

            var feedback = await ComputeFeedbackAsync(
                exercise.UserId,
                dto.MainAnswer!,
                dto.FollowUpAnswer!,
                cancellationToken);

            return new SubmitDistancedJournalResultDto(
                MapExercise(exercise),
                feedback);
        }

        public async Task<SubmitDistancedJournalResultDto> SubmitAnswerWithPhotosAsync(
            Guid userId,
            SubmitDistancedJournalAnswerDto dto,
            IReadOnlyCollection<PhotoUpload> photos,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.");

            if (photos.Count > MaxPhotosPerAnswer)
                throw new InvalidOperationException($"You can upload up to {MaxPhotosPerAnswer} photos.");

            var hasPhotos = photos.Count > 0;
            var hasText = !string.IsNullOrWhiteSpace(dto.MainAnswer)
                || !string.IsNullOrWhiteSpace(dto.FollowUpAnswer);

            if (!hasPhotos && !hasText)
                throw new InvalidOperationException("Provide at least text or photos.");

            if (hasText)
                ValidateTextAnswer(dto.MainAnswer, dto.FollowUpAnswer);

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Distanced journal exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var dailySession = await GetOrCreateTodaySessionAsync(exercise.UserId, cancellationToken);
            var submittedAt = _dateTimeProvider.UtcNow;

            foreach (var photo in photos)
            {
                var photoId = Guid.NewGuid();
                var objectKey = $"distanced-journals/{exercise.Id}/{photoId}";

                try
                {
                    await _photoStorage.SaveAsync(
                        objectKey,
                        photo.Content,
                        photo.SizeBytes,
                        photo.ContentType,
                        cancellationToken);
                }
                finally
                {
                    photo.Content.Dispose();
                }

                var entity = new DistancedJournalPhoto(
                    photoId,
                    objectKey,
                    photo.FileName,
                    photo.ContentType,
                    photo.SizeBytes,
                    submittedAt);

                exercise.AddPhoto(entity, MaxPhotosPerAnswer);
            }

            exercise.SubmitAnswer(
                dto.MainAnswer,
                dto.FollowUpAnswer,
                dto.Reflection,
                submittedAt);

            dailySession.RecordExercise(
                exercise.Id,
                ExerciseType.DistancedJournal,
                submittedAt);

            await SaveSessionWithRetryAsync(
                dailySession,
                exercise.Id,
                ExerciseType.DistancedJournal,
                submittedAt,
                cancellationToken);

            ThirdPersonFeedbackType? feedback = hasPhotos
                ? null
                : await ComputeFeedbackAsync(
                    exercise.UserId,
                    dto.MainAnswer!,
                    dto.FollowUpAnswer!,
                    cancellationToken);

            return new SubmitDistancedJournalResultDto(
                MapExercise(exercise),
                feedback);
        }

        public async Task<(Stream Stream, string ContentType, string FileName)> GetPhotoAsync(
            Guid userId,
            Guid exerciseId,
            Guid photoId,
            CancellationToken cancellationToken = default)
        {
            var exercise = await _exerciseRepository.GetByIdAsync(exerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Distanced journal exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var photo = exercise.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo is null)
                throw new InvalidOperationException("Photo was not found.");

            var stream = await _photoStorage.OpenReadAsync(photo.ObjectKey, cancellationToken);
            return (stream, photo.ContentType, photo.FileName);
        }

        public async Task<DistancedJournalExerciseDto> AddReflectionAsync(
            Guid userId,
            AddDistancedJournalReflectionDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.");

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Distanced journal exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var dailySession = await GetOrCreateTodaySessionAsync(exercise.UserId, cancellationToken);

            exercise.AddReflection(dto.Reflection);

            dailySession.RecordExercise(
                exercise.Id,
                ExerciseType.DistancedJournalReflection,
                _dateTimeProvider.UtcNow);

            await SaveSessionWithRetryAsync(
                dailySession,
                exercise.Id,
                ExerciseType.DistancedJournalReflection,
                _dateTimeProvider.UtcNow,
                cancellationToken);

            return MapExercise(exercise);
        }

        private static DistancedJournalChallengeDto MapChallenge(DistancedJournalChallenge challenge)
        {
            return new DistancedJournalChallengeDto(
                challenge.Id,
                challenge.Content,
                challenge.FollowUpQuestion,
                challenge.ChallengeLevel,
                challenge.SkillId);
        }

        private static DistancedJournalExerciseDto MapExercise(DistancedJournalExercise exercise)
        {
            return new DistancedJournalExerciseDto(
                exercise.Id,
                exercise.UserId,
                exercise.ChallengeId,
                exercise.Answer?.MainAnswer,
                exercise.Answer?.FollowUpAnswer,
                exercise.Answer?.Reflection,
                exercise.Answer?.SubmittedAt,
                exercise.IsCompleted(),
                BuildPhotoUrls(exercise));
        }

        private async Task<DailySession> GetOrCreateTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var today = _dateTimeProvider.UtcNow.Date;

            var session = await _dailySessionRepository.GetByUserAndDateAsync(
                userId,
                today,
                cancellationToken);

            if (session is not null)
                return session;

            session = new DailySession(Guid.NewGuid(), userId, today);
            await _dailySessionRepository.AddAsync(session, cancellationToken);
            await _dailySessionRepository.SaveChangesAsync(cancellationToken);

            return session;
        }

        public async Task<DistancedJournalChallengeDto> GetRandomChallengeAsync(
            ChallengeLevel level,
            CancellationToken cancellationToken = default)
        {
            var challenge = await _challengeRepository.GetRandomByLevelAsync(level, cancellationToken);

            if (challenge is null)
                throw new InvalidOperationException("No challenges found for the selected level.");

            return MapChallenge(challenge);
        }

        private static IReadOnlyCollection<string> BuildPhotoUrls(DistancedJournalExercise exercise)
        {
            return exercise.Photos
                .Select(p => $"/api/DistancedJournals/{exercise.Id}/photos/{p.Id}")
                .ToList();
        }

        private static void ValidateTextAnswer(string? mainAnswer, string? followUpAnswer)
        {
            if (string.IsNullOrWhiteSpace(mainAnswer) || string.IsNullOrWhiteSpace(followUpAnswer))
                throw new ArgumentException("Main and follow-up answers must be provided together.");
        }

        private async Task SaveSessionWithRetryAsync(
            DailySession session,
            Guid exerciseId,
            ExerciseType exerciseType,
            DateTime timestamp,
            CancellationToken cancellationToken)
        {
            try
            {
                await _dailySessionRepository.UpdateAsync(session, cancellationToken);
                await _dailySessionRepository.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var fresh = await _dailySessionRepository.ReloadAsync(session.Id, cancellationToken);
                if (fresh is null)
                    throw;

                var alreadyRecorded = fresh.Events
                    .OfType<ExerciseRecord>()
                    .Any(e => e.ExerciseId == exerciseId && e.Type == exerciseType);

                if (alreadyRecorded)
                    return;

                fresh.RecordExercise(exerciseId, exerciseType, timestamp);
                await _dailySessionRepository.UpdateAsync(fresh, cancellationToken);
                await _dailySessionRepository.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task<ThirdPersonFeedbackType> ComputeFeedbackAsync(
            Guid userId,
            string mainAnswer,
            string followUpAnswer,
            CancellationToken cancellationToken)
        {
            var fullText = $"{mainAnswer} {followUpAnswer}";
            var firstName = await _userProfileReader.GetFirstNameAsync(userId, cancellationToken);
            var metric = _thirdPersonAnalyzer.Analyze(fullText, "sr", firstName);
            return _thirdPersonAnalyzer.GetFeedback(metric).FeedbackType;
        }

    }
}
