using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public abstract class SessionEvent
    {
        public DateTime Timestamp { get; protected set; }

        protected SessionEvent() { }

        protected SessionEvent(DateTime timestamp)
        {
            Timestamp = timestamp;
        }
    }
}
