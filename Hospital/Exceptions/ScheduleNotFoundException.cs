using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    class ScheduleNotFoundException : Exception
    {
        public ScheduleNotFoundException(string message) : base(message){}
    }
}
