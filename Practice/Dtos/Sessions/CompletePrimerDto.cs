using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record CompletePrimerDto(
        bool IsSkipped,
        List<Guid>? PresentedStatementIds = null,
        Guid? SelectedStatementId = null,
        Guid? GrowthMessageId = null
    );
}
