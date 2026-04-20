using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime BusinessDate { get; }
    }
}
