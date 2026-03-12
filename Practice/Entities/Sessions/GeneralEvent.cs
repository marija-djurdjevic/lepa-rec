using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public class GeneralEvent : SessionEvent
    {
        public string EventType { get; private set; } = string.Empty;

        private GeneralEvent() { }

        public GeneralEvent(string eventType, DateTime timestamp) : base(timestamp)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("Tip događaja je obavezan.", nameof(eventType));

            EventType = eventType;
        }
    }
}
