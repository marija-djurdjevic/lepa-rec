using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
