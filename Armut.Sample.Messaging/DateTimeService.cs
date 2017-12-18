using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging
{
    internal class DateTimeService : IDateTimeService
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
