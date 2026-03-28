using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public class DailySession : Entity<Guid>, IAggregateRoot
    {
        private readonly List<SessionEvent> _events = new();

        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public SessionStatus Status { get; private set; }

        public MindsetPrimerResult? PrimerResult { get; private set; }

        public IReadOnlyCollection<SessionEvent> Events => _events;
        public bool RequiresPrimer => PrimerResult == null;
        public bool PrimerCompleted => PrimerResult != null && !PrimerResult.IsSkipped;
        public bool PrimerSkipped => PrimerResult != null && PrimerResult.IsSkipped;
        public bool HasRecordedExercises => _events.OfType<ExerciseRecord>().Any();
        public int CompletedExercisesCount => _events.OfType<ExerciseRecord>().Count();

        private DailySession() : base() { }

        public DailySession(Guid id, Guid userId, DateTime date) : base(id)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId ne može biti prazan.", nameof(userId));

            UserId = userId;
            Date = date.Date;
            Status = SessionStatus.Started;
        }

        public void CompletePrimer(
            IEnumerable<Guid> presentedStatementIds,
            Guid selectedStatementId,
            Guid growthMessageId,
            DateTime timestamp)
        {
            EnsureSessionIsActive();
            EnsurePrimerNotAlreadyRecorded();

            if (growthMessageId == Guid.Empty)
                throw new ArgumentException("GrowthMessageId ne može biti prazan.", nameof(growthMessageId));

            PrimerResult = new MindsetPrimerResult(
                presentedStatementIds,
                selectedStatementId,
                growthMessageId,
                false,
                timestamp);

            MoveToInProgressIfNeeded();
            AddGeneralEvent("PrimerCompleted", timestamp);
        }

        public void SkipPrimer(DateTime timestamp)
        {
            EnsureSessionIsActive();
            EnsurePrimerNotAlreadyRecorded();

            PrimerResult = new MindsetPrimerResult(
                new List<Guid>(), 
                null,
                null,
                true,
                timestamp);

            MoveToInProgressIfNeeded();
            AddGeneralEvent("PrimerSkipped", timestamp);
        }

        public void RecordExercise(Guid exerciseId, ExerciseType type, DateTime timestamp)
        {
            EnsureSessionIsActive();

            if (RequiresPrimer)
                throw new InvalidOperationException("Nije moguće evidentirati vježbu prije mindset primera.");

            if (exerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId ne može biti prazan.", nameof(exerciseId));

            _events.Add(new ExerciseRecord(exerciseId, type, timestamp));
            MoveToInProgressIfNeeded();
        }

        public void Complete(DateTime timestamp)
        {
            if (Status == SessionStatus.Completed)
                return;

            if (Status == SessionStatus.Abandoned)
                throw new InvalidOperationException("Napuštena sesija ne može biti završena.");

            if (RequiresPrimer)
                throw new InvalidOperationException("Sesija ne može biti završena prije mindset primera.");

            if (!HasRecordedExercises)
                throw new InvalidOperationException("Sesija ne može biti završena bez evidentirane vježbe.");

            Status = SessionStatus.Completed;
            AddGeneralEvent("SessionCompleted", timestamp);
        }

        public void Abandon(DateTime timestamp)
        {
            if (Status == SessionStatus.Completed)
                throw new InvalidOperationException("Završena sesija ne može biti napuštena.");

            if (Status == SessionStatus.Abandoned)
                throw new InvalidOperationException("Sesija je već označena kao napuštena.");

            Status = SessionStatus.Abandoned;
            AddGeneralEvent("SessionAbandoned", timestamp);
        }

        private void EnsurePrimerNotAlreadyRecorded()
        {
            if (PrimerResult != null)
                throw new InvalidOperationException("Mindset primer je već zabilježen za današnju sesiju.");
        }

        private void EnsureSessionIsActive()
        {
            if (Status == SessionStatus.Completed)
                throw new InvalidOperationException("Nije moguće mijenjati završenu sesiju.");

            if (Status == SessionStatus.Abandoned)
                throw new InvalidOperationException("Nije moguće mijenjati napuštenu sesiju.");
        }

        private void MoveToInProgressIfNeeded()
        {
            if (Status == SessionStatus.Started)
                Status = SessionStatus.InProgress;
        }

        private void AddGeneralEvent(string eventType, DateTime timestamp)
        {
            _events.Add(new GeneralEvent(eventType, timestamp));
        }
    }
}
