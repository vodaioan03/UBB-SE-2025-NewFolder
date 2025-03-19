using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    class InvalidAppointmentException : Exception
    {
        public InvalidAppointmentException(string message) : base(message)
        {
        }
    }
}
