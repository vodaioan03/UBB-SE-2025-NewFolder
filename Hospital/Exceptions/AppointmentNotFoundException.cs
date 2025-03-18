using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    class AppointmentNotFoundException : Exception
    {
        public AppointmentNotFoundException(string message) : base(message){}
    }
}
