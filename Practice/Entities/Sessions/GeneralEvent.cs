using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public record GeneralEvent(string EventType, DateTime Timestamp) : SessionEvent(Timestamp);
}
