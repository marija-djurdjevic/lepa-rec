using AngularNetBase.Practice.Entities.Sessions;
using System;

namespace AngularNetBase.Practice.Entities.Scheduling
{
    public class UserDailyPracticeAssignment
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public ExerciseType MainExerciseType { get; private set; }
        public Guid? DistancedJournalChallengeId { get; private set; }
        public Guid? DistancedJournalChallengeId2 { get; private set; }
        public Guid? PerspectiveScenarioChallengeId { get; private set; }
        public Guid? PerspectiveScenarioChallengeId2 { get; private set; }
        public Guid? ReflectionExerciseId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private UserDailyPracticeAssignment() { }

        public UserDailyPracticeAssignment(
            Guid id,
            Guid userId,
            DateTime date,
            ExerciseType mainExerciseType,
            Guid? distancedJournalChallengeId,
            Guid? distancedJournalChallengeId2,
            Guid? perspectiveScenarioChallengeId,
            Guid? perspectiveScenarioChallengeId2,
            Guid? reflectionExerciseId,
            DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be provided.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.", nameof(userId));

            if (mainExerciseType == ExerciseType.DistancedJournalReflection)
                throw new ArgumentException("Reflection cannot be the main exercise type.", nameof(mainExerciseType));

            Id = id;
            UserId = userId;
            Date = date.Date;
            MainExerciseType = mainExerciseType;
            DistancedJournalChallengeId = distancedJournalChallengeId;
            DistancedJournalChallengeId2 = distancedJournalChallengeId2;
            PerspectiveScenarioChallengeId = perspectiveScenarioChallengeId;
            PerspectiveScenarioChallengeId2 = perspectiveScenarioChallengeId2;
            ReflectionExerciseId = reflectionExerciseId;
            CreatedAt = createdAt;
        }
    }
}
