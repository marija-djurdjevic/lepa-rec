using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime BusinessDate
        {
            get
            {
                var localNow = TimeZoneInfo.ConvertTimeFromUtc(UtcNow, _timeZone);
                var businessDate = localNow.Hour < 3
                    ? localNow.Date.AddDays(-1)
                    : localNow.Date;

                return businessDate;
            }
        }

        private readonly TimeZoneInfo _timeZone = ResolveTimeZone();

        private static TimeZoneInfo ResolveTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Sarajevo");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.Local;
                }
            }
        }
    }
}
