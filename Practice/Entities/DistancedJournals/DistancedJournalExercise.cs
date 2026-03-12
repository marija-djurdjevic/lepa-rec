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
        public DistancedJournalAnswer? Answer { get; private set; }

        private DistancedJournalExercise() : base() { }

        public DistancedJournalExercise(Guid id, Guid userId, Guid challengeId) : base(id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.");

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be a valid GUID.");

            if (challengeId == Guid.Empty)
                throw new ArgumentException("Challenge id must be a valid GUID.");

            UserId = userId;
            ChallengeId = challengeId;
        }

        public void SubmitAnswer(string mainAnswer, string followUpAnswer, string? reflection)
        {
            if (Answer is not null)
                throw new InvalidOperationException("Answer has already been submitted.");

            Answer = new DistancedJournalAnswer(
                mainAnswer,
                followUpAnswer,
                reflection,
                DateTime.UtcNow);
        }

        public void AddReflection(string reflection)
        {
            if (Answer is null)
                throw new InvalidOperationException("Cannot add reflection before submitting the answer.");

            Answer.AddReflection(reflection);
        }

        public bool IsCompleted()
        {
            return Answer is not null;
        }
    }
}
