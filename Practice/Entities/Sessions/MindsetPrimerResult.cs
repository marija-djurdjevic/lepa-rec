using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public class MindsetPrimerResult
    {
        public Guid? AffirmationValueId { get; private set; }
        public Guid? GrowthMessageId { get; private set; }
        public bool IsSkipped { get; private set; }
        public DateTime Timestamp { get; private set; }

        private MindsetPrimerResult() { }

        public MindsetPrimerResult(Guid? affirmationValueId, Guid? growthMessageId, bool isSkipped, DateTime timestamp)
        {
            AffirmationValueId = affirmationValueId;
            GrowthMessageId = growthMessageId;
            IsSkipped = isSkipped;
            Timestamp = timestamp;
        }
    }
}
