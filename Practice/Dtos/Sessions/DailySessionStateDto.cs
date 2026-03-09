using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.Sessions
{
    public record DailySessionStateDto(Guid SessionId, bool RequiresPrimer, string Status);
}
