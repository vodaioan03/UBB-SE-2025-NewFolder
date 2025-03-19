using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    class AppointmentConflictException : Exception
    {
        public AppointmentConflictException(string message) : base(message)
        {
        }
    }
}
