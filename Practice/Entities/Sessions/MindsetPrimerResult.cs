using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public class MindsetPrimerResult
    {
        public List<Guid> PresentedStatementIds { get; private set; } = new();

        public Guid? SelectedStatementId { get; private set; }

        public Guid? GrowthMessageId { get; private set; }

        public bool IsSkipped { get; private set; }

        public DateTime Timestamp { get; private set; }

        private MindsetPrimerResult() { }

        public MindsetPrimerResult(
            IEnumerable<Guid> presentedStatementIds,
            Guid? selectedStatementId,
            Guid? growthMessageId,
            bool isSkipped,
            DateTime timestamp)
        {
            var distinctIds = presentedStatementIds != null
                ? presentedStatementIds.Where(id => id != Guid.Empty).Distinct().ToList()
                : new List<Guid>();

            if (isSkipped)
            {
                if (selectedStatementId.HasValue)
                    throw new ArgumentException("Selected statement must be null when primer is skipped.", nameof(selectedStatementId));

                if (growthMessageId.HasValue)
                    throw new ArgumentException("Growth message must be null when primer is skipped.", nameof(growthMessageId));
            }
            else
            {
                if (distinctIds.Count == 0)
                    throw new ArgumentException("At least one presented statement is required when not skipped.", nameof(presentedStatementIds));

                if (!selectedStatementId.HasValue || selectedStatementId.Value == Guid.Empty)
                    throw new ArgumentException("Selected statement is required when primer is not skipped.", nameof(selectedStatementId));

                if (!distinctIds.Contains(selectedStatementId.Value))
                    throw new ArgumentException("Selected statement must be one of the presented statements.", nameof(selectedStatementId));

                if (!growthMessageId.HasValue || growthMessageId.Value == Guid.Empty)
                    throw new ArgumentException("Growth message is required when primer is not skipped.", nameof(growthMessageId));
            }

            PresentedStatementIds = distinctIds;
            SelectedStatementId = selectedStatementId;
            GrowthMessageId = growthMessageId;
            IsSkipped = isSkipped;
            Timestamp = timestamp;
        }
    }
}
