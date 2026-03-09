using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record CompletePrimerDto(bool IsSkipped, Guid? AffirmationValueId = null, Guid? GrowthMessageId = null);
}
