using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public record MindsetPrimerResult(Guid? AffirmationValueId, Guid? GrowthMessageId, bool IsSkipped, DateTime Timestamp);
}
