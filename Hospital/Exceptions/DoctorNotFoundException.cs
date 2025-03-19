using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
  class DoctorNotFoundException : Exception
  {
    public DoctorNotFoundException(string message) : base(message)
    {
    }
  }
}
