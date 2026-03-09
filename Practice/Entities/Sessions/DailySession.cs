using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public class DailySession : Entity<Guid>, IAggregateRoot
    {
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public SessionStatus Status { get; private set; }

        public MindsetPrimerResult? PrimerResult { get; private set; }

        private readonly List<SessionEvent> _events = new();
        public IReadOnlyCollection<SessionEvent> Events => _events.AsReadOnly();

        private DailySession() : base() { } 

        public DailySession(Guid id, Guid userId) : base(id)
        {
            UserId = userId;
            Date = DateTime.UtcNow.Date; 
            Status = SessionStatus.Started;
        }

        public void RecordPrimerCompleted(bool isSkipped, Guid? affirmationId = null, Guid? growthMessageId = null)
        {
            if (PrimerResult != null)
                throw new InvalidOperationException("Vježba disanja je već zabilježena za danas.");

            PrimerResult = new MindsetPrimerResult(affirmationId, growthMessageId, isSkipped, DateTime.UtcNow);
            Status = SessionStatus.InProgress;

            _events.Add(new GeneralEvent(isSkipped ? "PrimerSkipped" : "PrimerCompleted", DateTime.UtcNow));
        }
    }
}
