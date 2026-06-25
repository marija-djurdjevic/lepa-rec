using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalExercise : Entity<Guid>, IAggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid ChallengeId { get; private set; }
        public bool IsOnboardingHookRun { get; private set; }
        public DistancedJournalAnswer? Answer { get; private set; }
        private readonly List<DistancedJournalPhoto> _photos = new();
        public IReadOnlyCollection<DistancedJournalPhoto> Photos => _photos.AsReadOnly();

        private DistancedJournalExercise() : base() { }

        public DistancedJournalExercise(Guid id, Guid userId, Guid challengeId, bool isOnboardingHookRun = false) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.");

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be a valid GUID.");

            if (challengeId == Guid.Empty)
                throw new ArgumentException("Challenge id must be a valid GUID.");

            UserId = userId;
            ChallengeId = challengeId;
            IsOnboardingHookRun = isOnboardingHookRun;
        }

        public void SubmitAnswer(string? mainAnswer, string? followUpAnswer, string? reflection, DateTime submittedAt)
        {
            if (Answer is not null)
                throw new InvalidOperationException("Answer has already been submitted.");

            Answer = new DistancedJournalAnswer(
                mainAnswer,
                followUpAnswer,
                reflection,
                submittedAt);
        }

        public void AddPhoto(DistancedJournalPhoto photo, int maxPhotos)
        {
            if (photo is null)
                throw new ArgumentNullException(nameof(photo));

            if (_photos.Count >= maxPhotos)
                throw new InvalidOperationException($"Cannot add more than {maxPhotos} photos.");

            _photos.Add(photo);
        }

        public void AddReflection(string reflection)
        {
            if (Answer is null)
                throw new InvalidOperationException("Cannot add reflection before submitting the answer.");

            Answer.AddReflection(reflection);
        }

        public void SetGeneratedReflectionQuestion(string question)
        {
            if (Answer is null)
                throw new InvalidOperationException("Cannot add a generated reflection question before submitting the answer.");

            Answer.SetGeneratedReflectionQuestion(question);
        }

        public void AddGeneratedReflectionAnswer(string answer)
        {
            if (Answer is null)
                throw new InvalidOperationException("Cannot add a generated reflection answer before submitting the answer.");

            Answer.AddGeneratedReflectionAnswer(answer);
        }

        public bool IsCompleted()
        {
            return Answer is not null;
        }
    }
}
