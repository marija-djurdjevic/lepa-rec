using AngularNetBase.Practice.Dtos.DistancedJournals;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.Sessions;
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

        public DistancedJournalService(
            IDistancedJournalChallengeRepository challengeRepository,
            IDistancedJournalExerciseRepository exerciseRepository,
            ISessionRepository sessionRepository)
        {
            _challengeRepository = challengeRepository;
            _exerciseRepository = exerciseRepository;
            _dailySessionRepository = sessionRepository;
        }

        public async Task<DistancedJournalChallengeDto> CreateChallengeAsync(
            CreateDistancedJournalChallengeDto dto,
            CancellationToken cancellationToken = default)
        {
            var challenge = new DistancedJournalChallenge(
                Guid.NewGuid(),
                dto.Content,
                dto.FollowUpQuestion,
                dto.ChallengeLevel);

            await _challengeRepository.AddAsync(challenge, cancellationToken);

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

            return MapExercise(exercise);
        }

        public async Task<DistancedJournalExerciseDto?> GetExerciseByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Exercise id must be provided.");

            var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
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

        public async Task<DistancedJournalExerciseDto> SubmitAnswerAsync(
            SubmitDistancedJournalAnswerDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.");

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Distanced journal exercise was not found.");

            var dailySession = await _dailySessionRepository.GetByUserAndDateAsync(
                exercise.UserId,
                dto.SessionDate.Date,
                cancellationToken);

            if (dailySession is null)
                throw new InvalidOperationException("Daily session was not found.");

            exercise.SubmitAnswer(
                dto.MainAnswer,
                dto.FollowUpAnswer,
                dto.Reflection);

            dailySession.RecordExercise(
                exercise.Id,
                ExerciseType.DistancedJournal,
                DateTime.UtcNow);

            await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
            await _dailySessionRepository.UpdateAsync(dailySession, cancellationToken);
            await _dailySessionRepository.SaveChangesAsync(cancellationToken);

            return MapExercise(exercise);
        }

        public async Task<DistancedJournalExerciseDto> AddReflectionAsync(
            AddDistancedJournalReflectionDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.");

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Distanced journal exercise was not found.");

            exercise.AddReflection(dto.Reflection);

            await _exerciseRepository.UpdateAsync(exercise, cancellationToken);

            return MapExercise(exercise);
        }

        private static DistancedJournalChallengeDto MapChallenge(DistancedJournalChallenge challenge)
        {
            return new DistancedJournalChallengeDto(
                challenge.Id,
                challenge.Content,
                challenge.FollowUpQuestion,
                challenge.ChallengeLevel);
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
                exercise.IsCompleted());
        }
    }
}